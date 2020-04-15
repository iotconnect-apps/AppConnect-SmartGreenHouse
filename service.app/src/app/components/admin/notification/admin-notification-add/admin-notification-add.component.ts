import { Component, OnInit } from '@angular/core';
import { DeviceService, RuleService, NotificationService, GatewayService } from 'app/services';
import { NgxSpinnerService } from 'ngx-spinner';
import { FormControl, FormArray, FormGroup, Validators } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router';
import { Notification } from 'app/services/notification/notification.service';
import * as _ from "lodash";
import { upperCase } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'app-admin-notification-add',
  templateUrl: './admin-notification-add.component.html',
  styleUrls: ['./admin-notification-add.component.css']
})
export class AdminNotificationAddComponent implements OnInit {

  templateList = [];
  attributeList = [];
  conditionList = [];
  severityList = [];
  commandList = [];
  postForm: FormGroup;
  checkSubmitStatus = false;
  isEdit = false;
  ruleType = '1';
  notificationGuid = '';
  notificationDetail = {};
  deviceCommandType = false;
  formLoad = false;
  pageTile = "ADD NOTIFICATION";
  myNotification: any = [
    {
      name: "DeviceCommand",
      value: "devicecommand",
      binary: 4,
      index: 2

    },
    {
      name: "Email",
      value: "email",
      binary: 8,
      index: 3
    },

    {
      name: "Push",
      value: "push",
      binary: 16,
      index: 4
    },

    {
      name: "SignalR",
      value: "singalr",
      binary: 64,
      index: 6
    },
    // {
    //   name: "WebHook",
    //   value: "webhook",
    //   binary: 128,
    //   index: 7
    // },
    {
      name: "UI Alert",
      value: "uialert",
      binary: 256,
      index: 8
    },
    {
      name: "MQTT",
      value: "mqtt",
      binary: 512,
      index: 9
    },



  ];
  selectedNotification: [string];
  attributeGuidValidationMsg = false;
  condtionValueValidationMsg = false;
  condtionValueValidationMsgText = ';'
  commandTextValidationMsg = false;
  commandValueValidationMsg = false;
  commandValueValidationMsgText = '';
  notificationTypeValidationMsg = false;
  constructor(
    private gatewayService: GatewayService,
    private ruleService: RuleService,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
      if (params['notificationGuid'] !== 'add') {
        this.isEdit = true;
        this.pageTile = "EDIT NOTIFICATION";
        this.notificationGuid = params['notificationGuid'];
        this.getNotificationDetail();

      } else {
        this.formLoad = true;
        this.getTemplateLookup();
        this.getSeveritylevelLookup();
        this.getConditionLookup();

      }
    });
    this.createFormGroup();
  }
  verifyCondition() {
    if (this.postForm.controls['conditionText'].value && this.postForm.controls['kittypeGuid'].value) {
      this.ruleService.verifyCondtion({ expression: this.postForm.controls['conditionText'].value, deviceTemplateGuid: this.postForm.controls['kittypeGuid'].value }).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this._notificationService.add(new Notification('success', "Condition verified successfully"));
        }
        else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
      });
    }
    else {
      if (this.postForm.controls['kittypeGuid'].value === '') {
        this._notificationService.add(new Notification('error', "please select template"));
      } else {
        this._notificationService.add(new Notification('error', "please enter condtions"));
      }
    }
  }


  checkIschecked(modulePermission, index) {
    if ((modulePermission & (1 << index)) >> index) {
      return true;
    } else {
      return false;
    }
  }

  clickAttribute(name, tag) {
    let val = this.postForm.controls['conditionText'].value;
    this.postForm.patchValue({ conditionText: val + tag + '#' + name });
  }
  getNotificationDetail() {
    this.spinner.show();
    this.ruleService.getRuleDetail(this.notificationGuid).subscribe(response => {
      // this.spinner.hide();
      let that = this;
      let dataNotifcation = [];
      if (response.isSuccess === true) {
        this.notificationDetail = response.data;
        this.myNotification.forEach(function (element, i) {
          element['selected'] = that.checkIschecked(that.notificationDetail['notificationType'], element['index']);
          dataNotifcation.push(element['selected']);
        });
        this.getSelectedNotification();
        this.postForm.patchValue({ notificationTypes: dataNotifcation })
        that.selectedNotification = this.postForm.controls['notificationTypes'].value;
        this.deviceCommandType = dataNotifcation[0];
        //that.selectedNotification.push(element['selected']);
        //
        this.createFormGroup();
        this.formLoad = true;
        this.postForm.patchValue({
          name: this.notificationDetail['name'],
          conditionValue: this.notificationDetail['conditionValue'],
          ruleType: this.notificationDetail['ruleType'].toString(),
          description: this.notificationDetail['description'],
        })
        this.ruleType = this.notificationDetail['ruleType'].toString();
        this.getTemplateLookup();
        this.getSeveritylevelLookup();
        this.getConditionLookup();
        this.getTemplateAttributeLookup();
      } else {
        this.router.navigate(['/admin/notifications']);
        this._notificationService.add(new Notification('error', 'Notification not found'));
      }
    }, error => {
      this.spinner.hide();
      this.router.navigate(['/admin/notifications']);
      this._notificationService.add(new Notification('error', 'Notification not found'));
    });
  }

  onType(controlName = '') {
    console.log('this.postForm',this.postForm);
    
    if (controlName === 'attributeGuid') {
      if (this.postForm.controls['attributeGuid'].value === '') {
        this.attributeGuidValidationMsg = true;
      } else {
        this.attributeGuidValidationMsg = false;
      }
    } else if (controlName === 'conditionValue') {
      if (this.postForm.controls['conditionValue'].value === '') {
        this.condtionValueValidationMsg = true;
        this.condtionValueValidationMsgText = 'Please enter value';
      } else {
        if (isNaN(this.postForm.controls['conditionValue'].value)) {
          this.condtionValueValidationMsg = true;
          this.condtionValueValidationMsgText = 'Please enter only numbers';
        } else {
          this.condtionValueValidationMsg = false;
          this.condtionValueValidationMsgText = '';
        }


      }
    } else if (controlName === 'commandText') {
      if (this.postForm.controls['commandText'].value === '') {
        this.commandTextValidationMsg = true;
      } else {
        this.commandTextValidationMsg = false;
      }
    } else if (controlName === 'commandValue') {
      if (this.postForm.controls['commandValue'].value === '') {
        this.commandValueValidationMsg = true;
        this.commandValueValidationMsgText = 'Please enter value';
      } else {
        this.commandValueValidationMsg = false;
        if (isNaN(this.postForm.controls['commandValue'].value)) {
          this.commandValueValidationMsg = true;
          this.commandValueValidationMsgText = 'Please enter only numbers';
        } else {
          this.commandValueValidationMsg = false;
          this.commandValueValidationMsgText = '';
        }
      }
    }


  }

  createFormGroup() {
    this.postForm = new FormGroup({
      kittypeGuid: new FormControl('', [Validators.required]),
      ruleType: new FormControl('', [Validators.required]),
      name: new FormControl('', [Validators.required]),
      description: new FormControl('', [Validators.required]),
      conditionText: new FormControl('', [Validators.required]),
      conditionValue: new FormControl(''),
      severityLevelGuid: new FormControl('', [Validators.required]),
      attributeGuid: new FormControl(''),
      notificationTypes: this.createNotifications(this.myNotification),
      commandText: new FormControl(''),
      commandValue: new FormControl(''),
    });

    if (!this.isEdit) {
      this.postForm.patchValue({ ruleType: this.ruleType })
    }

  }

  getSelectedNotification() {
    this.selectedNotification = _.map(
      this.postForm.controls.notificationTypes["controls"],
      (note, i) => {
        if (this.myNotification[i]['name'] === 'DeviceCommand') {
          this.deviceCommandType = note.value;
          this.commandTextValidationMsg = false;
          this.commandValueValidationMsg = false;
        }

        return note.value && this.myNotification[i].value;
      }
    );
    this.getSelectedNotificationName();
  }

  getSelectedNotificationName() {
    this.selectedNotification = _.filter(
      this.selectedNotification,
      function (note) {
        if (note !== false) {
          return note;
        }
      }
    );
  }

  createNotifications(notesInputs) {
    const arr = notesInputs.map(note => {
      return new FormControl(note.selected || false);
    });
    return new FormArray(arr);
  }

  ruleTypeChange() {
    this.ruleType = this.postForm.controls['ruleType'].value;
    this.postForm.patchValue({
      conditionValue: '',
      conditionText: '',
      attributeGuid: ''
    });
    this.attributeGuidValidationMsg = false;
    this.condtionValueValidationMsg = false;
  }
  submitForm() {
    this.checkSubmitStatus = true;
    let typeOneValidation = true;
    if (this.ruleType === '1') {
      if (this.postForm.controls["attributeGuid"].value === '' || this.postForm.controls["conditionValue"].value === '' || isNaN(this.postForm.controls['conditionValue'].value)) {
        typeOneValidation = false;
        if (this.postForm.controls["attributeGuid"].value === '') {
          this.attributeGuidValidationMsg = true;
        }

        if (this.postForm.controls['conditionValue'].value === '') {
          this.condtionValueValidationMsg = true;
          this.condtionValueValidationMsgText = 'Please enter value';
        }
        if (isNaN(this.postForm.controls['conditionValue'].value)) {
          this.condtionValueValidationMsg = true;
          this.condtionValueValidationMsgText = 'Please enter only numbers';
        }

      } else {
        this.attributeGuidValidationMsg = false;
        this.condtionValueValidationMsg = false;
      }
    }
    if (this.deviceCommandType) {
      if (this.postForm.controls["commandText"].value === '' || this.postForm.controls["commandValue"].value === '' || isNaN(this.postForm.controls['commandValue'].value)) {
        typeOneValidation = false;
        if (this.postForm.controls["commandText"].value === '') {
          this.commandTextValidationMsg = true;
        }
        if (this.postForm.controls["commandValue"].value === '') {
          this.commandValueValidationMsg = true;
          this.commandValueValidationMsgText = 'Please enter value';
        }
        if (isNaN(this.postForm.controls['commandValue'].value)) {
          this.commandValueValidationMsg = true;
          this.commandValueValidationMsgText = 'Please enter only numbers';
        } 

      } else {
        this.commandTextValidationMsg = false;
        this.commandValueValidationMsg = false;
      }

    }
    this.notificationTypeValidationMsg = true;
    let that = this;
    this.postForm.controls['notificationTypes'].value.forEach(function (element, i) {
      if (element) {
        that.notificationTypeValidationMsg = false;
      }
    });
    if (this.notificationTypeValidationMsg) {
      typeOneValidation = false;
    }

    if (this.postForm.status === "VALID" && typeOneValidation) {
      this.spinner.show();
      let postData = this.postForm.value;
      if (this.isEdit) {
        postData["guid"] = this.notificationGuid;
      }
      postData["ruleType"] = parseInt(postData["ruleType"]);
      if (postData["ruleType"] === 2) {
        postData["attributeGuid"] = '';
        postData["conditionValue"] = '';
      }
      postData["notificationType"] = 0;
      let that = this;
      this.postForm.controls['notificationTypes'].value.forEach(function (element, i) {
        if (element) {
          postData["notificationType"] = postData["notificationType"] + that.myNotification[i]['binary'];
        }
      });

      if (postData["ruleType"] === 2) {
        if (this.postForm.controls['conditionText'].value && this.postForm.controls['kittypeGuid'].value) {
          this.ruleService.verifyCondtion({ expression: this.postForm.controls['conditionText'].value, deviceTemplateGuid: this.postForm.controls['kittypeGuid'].value }).subscribe(response => {

            if (response.isSuccess === true || 1) {
              this.ruleService.manageRule(postData).subscribe(response => {
                this.spinner.hide();
                if (response.isSuccess === true) {
                  this.router.navigate(['/admin/notifications']);
                  if (this.isEdit) {
                    this._notificationService.add(new Notification('success', "Notification has been updated successfully."));
                  } else {
                    this._notificationService.add(new Notification('success', "Notification has been added successfully."));
                  }
                }
                else {
                  this._notificationService.add(new Notification('error', response.message));
                }
              }, error => {
                this.spinner.hide();
              });
            }
            else {
              this.spinner.hide();
              this._notificationService.add(new Notification('error', response.message));
            }
          }, error => {
            this.spinner.hide();
            this.spinner.hide();
          });

        } else {
          if (this.postForm.controls['kittypeGuid'].value === '') {
            this._notificationService.add(new Notification('error', "please select template"));
          } else {
            this._notificationService.add(new Notification('error', "please enter condtions"));
          }
        }
      } else {
        this.ruleService.manageRule(postData).subscribe(response => {
          this.spinner.hide();
          if (response.isSuccess === true) {
            this.router.navigate(['/admin/notifications']);
            if (this.isEdit) {
              this._notificationService.add(new Notification('success', "Rule has been updated successfully."));
            } else {
              this._notificationService.add(new Notification('success', "Rule has been added successfully."));
            }
          }
          else {
            this._notificationService.add(new Notification('error', response.message));
          }
        }, error => {
          this.spinner.hide();
        });

      }

    }
  }

  getTemplateLookup() {
    this.gatewayService.getTemplateLookup().subscribe(response => {
      if (response.isSuccess === true) {
        this.templateList = response.data;
        if (this.isEdit && this.notificationDetail['kittypeGuid']) {
          this.postForm.patchValue({ kittypeGuid: (this.notificationDetail['kittypeGuid'].toUpperCase()) })
          this.getTemplateAttributeLookup();
        }
      } else {
        this.templateList = [];
      }

    }, error => {
      this.spinner.hide();
      this.templateList = [];
    });
  }

  getTemplateAttributeLookup() {
    if (this.postForm.controls['kittypeGuid'].value !== '') {
      this.spinner.show();
      this.gatewayService.getKitAttribueLookup(this.postForm.controls['kittypeGuid'].value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.attributeList = response.data;
          if (this.isEdit) {
            this.postForm.patchValue({ attributeGuid: (this.notificationDetail['attributeGuid']) })
          }
        } else {
          this.attributeList = [];
        }

      }, error => {
        this.spinner.hide();
        this.attributeList = [];
      });

      this.spinner.show();
      this.gatewayService.getTemplateCommandLookup(this.postForm.controls['kittypeGuid'].value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.commandList = response.data;
          if (this.isEdit && this.notificationDetail['commandText']) {
            this.postForm.patchValue({ commandText: (this.notificationDetail['commandText'].toUpperCase()) })
            this.postForm.patchValue({ commandValue: (this.notificationDetail['commandValue']) })
          }
        } else {
          this.commandList = [];
        }

      }, error => {
        this.spinner.hide();
        this.commandList = [];

      });
    }
  }

  getSeveritylevelLookup() {
    this.spinner.show();
    this.ruleService.getSeveritylevelLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.severityList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ severityLevelGuid: this.notificationDetail['severityLevelGuid'].toUpperCase() })
        }
      } else {
        this.severityList = [];
      }
    }, error => {
      this.spinner.hide();
      this.severityList = [];
    });
  }

  getConditionLookup() {
    this.spinner.show();
    this.ruleService.getConditionLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.conditionList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ conditionText: this.notificationDetail['conditionText'] })
        }
      } else {
        this.conditionList = [];
      }
    }, error => {
      this.spinner.hide();
      this.conditionList = [];
    });
  }

}
