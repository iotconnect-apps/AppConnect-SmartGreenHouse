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
  pageTile = "ADD NOTIFICATIONS";
  myNotification: any = [
    {
      name: "DeviceCommand",
      value: "devicecommand",
      binary: 1

    },
    {
      name: "Email",
      value: "email",
      binary: 2
    },
    {
      name: "Push",
      value: "push",
      binary: 4
    },
    {
      name: "SignalR",
      value: "singalr",
      binary: 8
    },
    {
      name: "WebHook",
      value: "webhook",
      binary: 16
    },
    {
      name: "UI Alert",
      value: "uialert",
      binary: 32
    },
    {
      name: "MQTT",
      value: "mqtt",
      binary: 64
    }
  ];
  selectedNotification: [string];
  attributeGuidValidationMsg = false;
  condtionValueValidationMsg = false;
  commandTextValidationMsg = false;
  commandValueValidationMsg = false;
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
        this.pageTile = "EDIT NOTIFICATIONS";
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

  checkIschecked(modulePermission, index) {
    if ((modulePermission & (1 << index)) >> index) {
      return true;
    } else {
      return false;
    }
  }

  clickAttribute(name) {
    let val = this.postForm.controls['conditionText'].value;
    this.postForm.patchValue({ conditionText: val + name });
  }
  getNotificationDetail() {
    this.spinner.show();
    this.ruleService.getRuleDetail(this.notificationGuid).subscribe(response => {
      this.spinner.hide();
      let that = this;
      let dataNotifcation = [];
      if (response.isSuccess === true) {
        this.notificationDetail = response.data;
        this.myNotification.forEach(function (element, i) {
          element['selected'] = that.checkIschecked(that.notificationDetail['notificationType'], i);
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
          ruleType: this.notificationDetail['ruleType'].toString()
        })
        this.ruleType = this.notificationDetail['ruleType'].toString();
        this.getTemplateLookup();
        this.getSeveritylevelLookup();
        this.getConditionLookup();
        this.getTemplateAttributeLookup();
      } else {
        this.router.navigate(['/admin/notification']);
        this._notificationService.add(new Notification('error', 'Notification not found'));
      }
    }, error => {
      this.spinner.hide();
      this.router.navigate(['/admin/notification']);
      this._notificationService.add(new Notification('error', 'Notification not found'));
    });
  }

  createFormGroup() {
    this.postForm = new FormGroup({
      templateGuid: new FormControl('', [Validators.required]),
      ruleType: new FormControl('', [Validators.required]),
      name: new FormControl('', [Validators.required]),
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
      if (this.postForm.controls["attributeGuid"].value === '' || this.postForm.controls["conditionValue"].value === '') {
        typeOneValidation = false;
        if (this.postForm.controls["attributeGuid"].value === '') {
          this.attributeGuidValidationMsg = true;
        }
        if (this.postForm.controls["conditionValue"].value === '') {
          this.condtionValueValidationMsg = true;
        }

      } else {
        this.attributeGuidValidationMsg = false;
        this.condtionValueValidationMsg = false;
      }
    }
    if (this.deviceCommandType) {
      if (this.postForm.controls["commandText"].value === '' || this.postForm.controls["commandValue"].value === '') {
        typeOneValidation = false;
        if (this.postForm.controls["commandText"].value === '') {
          this.commandTextValidationMsg = true;
        }
        if (this.postForm.controls["commandValue"].value === '') {
          this.commandValueValidationMsg = true;
        }

      } else {
        this.commandTextValidationMsg = false;
        this.commandValueValidationMsg = false;
      }

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
      this.ruleService.manageRule(this.postForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/admin/notification']);
          this._notificationService.add(new Notification('success', "Rule has been Addedd successfully."));
        }
        else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
      });

    }
  }

  getTemplateLookup() {
    this.gatewayService.getTemplateLookup().subscribe(response => {
      if (response.isSuccess === true) {
        this.templateList = response.data;
        if (this.isEdit) {
          this.postForm.patchValue({ templateGuid: (this.notificationDetail['templateGuid'].toUpperCase()) })
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
    if (this.postForm.controls['templateGuid'].value !== '') {

      this.gatewayService.getKitAttribueLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
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

      this.gatewayService.getTemplateCommandLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
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
