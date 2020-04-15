import { Component, OnInit, ViewChild } from '@angular/core'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatTableDataSource, MatSort, MatPaginator } from '@angular/material'
import { DeviceService, NotificationService, RuleService, GatewayService } from 'app/services';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { Notification } from 'app/services/notification/notification.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormArray, FormGroup, Validators } from '@angular/forms'
import * as _ from "lodash";

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.css']
})
export class NotificationListComponent implements OnInit {
  notificationList = [];
  adminNotificationList = [];
  searchText = '';
  deleteAlertDataModel: DeleteAlertDataModel;
  previceIndex = -1;
  isEdit = false;
  notificationDetail = {};
  notificationGuid = '';
  userList = [];
  roleList = [];
  entityList = [];

  deviceList = [];
  commandList = [];
  applyTo = '1';
  ruleType = '1';
  checkSubmitStatus = false;
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
    {
      name: "WebHook",
      value: "webhook",
      binary: 128,
      index: 7
    },
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
  deviceCommandType = false;
  entityValidationMsg = '';
  selectedNotification: [string];
  templateList = [];
  postForm: FormGroup;
  constructor(
    private spinner: NgxSpinnerService,
    public dialog: MatDialog,
    private _notificationService: NotificationService,
    private ruleService: RuleService,
    public _appConstant: AppConstant,
    private router: Router,
    private gatewayService: GatewayService,
  ) { }

  ngOnInit() {
    this.getAdminRuleList();
    this.createFormGroup();
    this.getUsersLookup();
    this.getRoleLookup();
    this.getGreenHouseLookup();
    this.getTemplateLookup();


  }
  getTemplateLookup() {
    this.spinner.show();
    this.gatewayService.getTemplateIotLookup().subscribe(response => {
      //this.spinner.hide();
      if (response.isSuccess === true) {
        this.templateList = response.data;
      } else {
        this.templateList = [];
      }

    }, error => {
      this.spinner.hide();
      this.templateList = [];
    });
  }
  createFormGroup() {
    this.postForm = new FormGroup({
      templateGuid: new FormControl('', [Validators.required]),
      ruleType: new FormControl('', [Validators.required]),
      applyTo: new FormControl('', [Validators.required]),
      name: new FormControl('', [Validators.required]),
      conditionText: new FormControl('', [Validators.required]),
      conditionValue: new FormControl(''),
      severityLevelGuid: new FormControl('', [Validators.required]),
      attributeGuid: new FormControl(''),
      notificationTypes: this.createNotifications(this.myNotification),
      commandText: new FormControl(''),
      commandValue: new FormControl(''),
      url: new FormControl(''),
      users: new FormControl(''),
      roles: new FormControl(''),
      entityGuid: new FormControl(''),
      devices: new FormControl(''),
      ignorePreference: new FormControl(''),

    });
  }
  createNotifications(notesInputs) {
    const arr = notesInputs.map(note => {
      return new FormControl(note.selected || false);
    });
    return new FormArray(arr);
  }

  clearForm() {
    this.isEdit = false;
    this.adminNotificationList[this.previceIndex]['selected'] = false;
    //this.postForm.reset();
  }
  submitForm() {
    this.checkSubmitStatus = true;
    let typeOneValidation = true;
    if (this.postForm.controls['applyTo'].value !== '3' && this.postForm.controls['entityGuid'].value === '') {
      typeOneValidation = false;
      this.entityValidationMsg = "Please select Green House";
    } else {
      this.entityValidationMsg = "";
    }
    if (this.postForm.status === "VALID" && typeOneValidation) {
      this.spinner.show();
      let postData = this.postForm.value;
      postData["ruleType"] = parseInt(postData["ruleType"]);
      if (postData["ruleType"] === 2) {
        postData["attributeGuid"] = '';
        postData["conditionValue"] = '';
      } else {
        let attributeInfo = postData["attributeGuid"];
        postData["attributeGuid"] = attributeInfo['guid'];
        postData["conditionText"] = attributeInfo['tag'] + '#' + attributeInfo['localName'] + ' ' + postData["conditionText"] + ' ' + postData["conditionValue"];
      }
      postData["applyTo"] = parseInt(postData["applyTo"]);
      if (postData["applyTo"] === 3) {
        postData["enityGuid"] = '';
      } else {
        postData["devices"] = [];
        var filterGHObj = this.entityList.filter(function (e) {
          return e.parentEntityGuid == null;
        });
        if (filterGHObj.length && filterGHObj[0]['value'] === postData['entityGuid']) {
          postData["applyTo"] = 2;
        }
      }
      postData["notificationType"] = 0;
      postData["deliveryMethod"] = [];
      let that = this;

      if (this.deviceCommandType) {
        postData['commandGuid'] = postData['commandText'];
        postData['parameterValue'] = postData['commandValue'];
      }
      this.postForm.controls['notificationTypes'].value.forEach(function (element, i) {
        if (element) {
          postData["notificationType"] = postData["notificationType"] + that.myNotification[i]['binary'];
          postData["deliveryMethod"].push(that.myNotification[i]['name']);
        }
      });
      if (postData["users"] === '') { postData["users"] = []; }
      if (postData["roles"] === '') { postData["roles"] = []; }
      postData["eventSubscriptionGuid"] = '';
      this.ruleService.manageUserRule(postData).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {

          this._notificationService.add(new Notification('success', "Notification has been added successfully."));
          this.adminNotificationList[this.previceIndex]['selected'] = false;
          this.isEdit = false;
          this.getRuleList();

        }
        else {
          if (response.message === "Rule name already exist") {
            response.message = "Notification name already exist";
            //this.postForm.reset();
            this.clearForm();
          }
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
      });

    }
  }
  selectRule(i, noteDetail) {
    this.postForm.patchValue({
      entityGuid: '',
      applyTo: ''
    });
    this.applyTo = '1';
    //this.postForm.reset();
    if (this.adminNotificationList[i]['selected'] !== undefined) {
      this.adminNotificationList[i]['selected'] = !this.adminNotificationList[i]['selected'];
      if (this.previceIndex !== i) {
        this.adminNotificationList[this.previceIndex]['selected'] = false;
      }
    } else {
      this.adminNotificationList[i]['selected'] = true;
      if (this.previceIndex !== -1) {
        this.adminNotificationList[this.previceIndex]['selected'] = false;
      }
    }

    this.isEdit = this.adminNotificationList[i]['selected'];
    this.notificationGuid = '';
    if (this.isEdit) {
      this.notificationGuid = this.adminNotificationList[i]['guid'];
      this.getNotificationDetail(noteDetail);
    }
    this.previceIndex = i;
  }
  checkIschecked(modulePermission, index) {
    if ((modulePermission & (1 << index)) >> index) {
      return true;
    } else {
      return false;
    }
  }
  getSelectedNotification() {
    this.selectedNotification = _.map(
      this.postForm.controls.notificationTypes["controls"],
      (note, i) => {
        return note.value && this.myNotification[i].value;
      }
    );
    this.getSelectedNotificationName();
  }
  onType(controlName = '') {
    if (controlName === 'entityGuid') {
      if (this.postForm.controls['entityGuid'].value === '') {
        this.entityValidationMsg = 'Please select Green House';
      } else {
        this.entityValidationMsg = '';
      }
    }


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
  getNotificationDetail(noteDetail) {
    let that = this;
    let dataNotifcation = [];
    this.notificationDetail = noteDetail;
    this.myNotification.forEach(function (element, i) {
      element['selected'] = that.checkIschecked(that.notificationDetail['notificationType'], element['index']);
      dataNotifcation.push(element['selected']);
    });
    this.getSelectedNotification();
    this.deviceCommandType = dataNotifcation[0];
    this.postForm.patchValue({ notificationTypes: dataNotifcation })
    this.postForm.patchValue({
      name: this.notificationDetail['name'],
      ruleType: this.notificationDetail['ruleType'].toString(),
      applyTo: this.applyTo,
      ignorePreference: this.notificationDetail['ignorePreference'],
      templateGuid: this.notificationDetail['kittypeGuid'],
      conditionText: this.notificationDetail['conditionText'],
      conditionValue: this.notificationDetail['conditionValue'],
      severityLevelGuid: this.notificationDetail['severityLevelGuid'],

    });
    if (this.deviceCommandType) {
      this.postForm.patchValue({
        commandText: this.notificationDetail['commandText'],
        commandValue: this.notificationDetail['commandValue'],
      });

    }
    this.templateList.forEach(element => {
      if (element.text === this.notificationDetail['kitTypeName']) {
        this.postForm.patchValue({ templateGuid: (element.value.toUpperCase()) })
      }

    })

    //this.getTemplateAttributeLookup(); 
    this.getTemplateAttributeLookup();

  }
  getTemplateAttributeLookup() {
    if (this.postForm.controls['templateGuid'].value !== '') {
      this.gatewayService.getTemplateAttribueIotLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
        if (response.isSuccess === true) {
          if (this.isEdit) {
            let that = this;
            var filterObj = response.data.filter(function (e) {
              return e.localName == that.notificationDetail['localName'] && e.tag == that.notificationDetail['tag'];
            });
            this.postForm.patchValue({ attributeGuid: filterObj[0] });

          }
        } else {

        }

      }, error => {
        this.spinner.hide();

      });

      this.gatewayService.getTemplateCommandIotLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
        if (response.isSuccess === true) {
          this.commandList = response.data;
          console.log('this.notificationDetail',this.notificationDetail);
          
          if (this.isEdit && this.notificationDetail['commandText']) {
            let that = this;
            var filterObj = response.data.filter(function (e) {
              return e.text == that.notificationDetail['commandName'];
            });
            
            
            this.postForm.patchValue({ commandText: filterObj[0]['value'] })
            this.postForm.patchValue({ commandValue: that.notificationDetail['commandValue'] })
          }
        } else {
          this.commandList = [];
        }

      }, error => {
        this.spinner.hide();
        this.commandList = [];

      });

      this.ruleService.getDeviceLookup(this.postForm.controls['templateGuid'].value).subscribe(response => {
        if (response.isSuccess === true) {
          this.deviceList = response.data;
          if (this.isEdit) {
            this.postForm.patchValue({ devices: (this.notificationDetail['devices']) })
          }
        } else {
          this.deviceList = [];
        }

      }, error => {
        this.spinner.hide();
        this.deviceList = [];

      });
    }
  }
  searchTextCallback($event) {
    this.searchText = $event;
    this.getRuleList();

  }
  deleteModel(RuleModel) {
    this.deleteAlertDataModel = {
      title: "Delete Notification",
      message: this._appConstant.msgConfirm.replace('modulename', RuleModel['name']),
      okButtonName: "Yes",
      cancelButtonName: "No",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteRule(RuleModel['guid']);
      }
    });
  }
  deleteRule(guid) {
    this.spinner.show();
    this.ruleService.deleteUserRule(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this._notificationService.add(new Notification('success', "Notification has been deleted successfully."));
        this.getRuleList();
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', 'Notification not found'));
    });
  }
  activeInactiveRule(id: string, isActive: boolean, name: string) {
    var status = isActive == true ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: name,
      modulename: "Notification"
    };
    this.deleteAlertDataModel = {
      title: "Change Notification Status",
      message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname/gi, function (matched) {
        return mapObj[matched];
      }),
      okButtonName: "Yes",
      cancelButtonName: "No",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.updateRuleStatus(id, isActive);

      }
    });

  }
  updateRuleStatus(guid, status) {
    this.spinner.show();
    this.ruleService.updateUserRuleStatus(guid, status).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this._notificationService.add(new Notification('success', "Notification status has been updated successfully."));
        this.getRuleList();
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
  getRuleList() {
    this.spinner.show();
    this.ruleService.getUserRuleList(this.searchText).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this.notificationList = response.data.items;
      } else {
        this.notificationList = [];
      }
    }, error => {
      this.spinner.hide();
      this.notificationList = [];
    });
  }
  ruleApplyONChange() {
    this.applyTo = this.postForm.controls['applyTo'].value;
  }
  checkEntiy() {
    if (this.applyTo === '1') {
      this.applyTo = '2'
    } else {
      this.applyTo = '1'
    }
    this.postForm.patchValue({ applyTo: this.applyTo });
  }
  getAdminRuleList() {
    this.spinner.show();
    this.ruleService.getRuleList(this.searchText).subscribe(response => {
      //this.spinner.hide();
      this.getRuleList();
      if (response.isSuccess && response.data.count) {
        this.adminNotificationList = response.data.items;
      } else {
        this.adminNotificationList = [];
      }
    }, error => {
      this.spinner.hide();
      this.adminNotificationList = [];
    });
  }
  getUsersLookup() {
    //this.spinner.show();
    this.ruleService.getUsersLookup().subscribe(response => {
      //this.spinner.hide();
      if (response.isSuccess === true) {
        this.userList = response.data;
      } else {
        this.userList = [];
      }
    }, error => {
      this.spinner.hide();
      this.userList = [];
    });
  }
  getRoleLookup() {
    //this.spinner.show();
    this.ruleService.getRoleLookup().subscribe(response => {
      // this.spinner.hide();
      if (response.isSuccess === true) {
        this.roleList = response.data;
      } else {
        this.roleList = [];
      }
    }, error => {
      this.spinner.hide();
      this.roleList = [];
    });
  }
  getGreenHouseLookup() {
    // this.spinner.show();
    this.ruleService.getGreenHouseLookup().subscribe(response => {
      //this.spinner.hide();
      if (response.isSuccess === true) {
        this.entityList = response.data;
      } else {
        this.entityList = [];
      }
    }, error => {
      this.spinner.hide();
      this.entityList = [];
    });
  }

}
