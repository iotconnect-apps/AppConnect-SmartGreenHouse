import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatTableDataSource, MatSort, MatPaginator } from '@angular/material'
import { DeviceService, NotificationService, RuleService } from 'app/services';
import { AppConstant, DeleteAlertDataModel } from "../../../../app.constants";
import { DeleteDialogComponent } from '../../../../components/common/delete-dialog/delete-dialog.component';
import { Notification } from 'app/services/notification/notification.service';

@Component({
  selector: 'app-admin-notification-list',
  templateUrl: './admin-notification-list.component.html',
  styleUrls: ['./admin-notification-list.component.css']
})
export class AdminNotificationListComponent implements OnInit {

  notificationList = [];
  searchText = '';
  deleteAlertDataModel: DeleteAlertDataModel;
  constructor(
    private spinner: NgxSpinnerService,
    public dialog: MatDialog,
    private _notificationService: NotificationService,
    private ruleService: RuleService,
    public _appConstant: AppConstant
  ) { }

  ngOnInit() {
    this.getRuleList();
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
    this.ruleService.deleteRule(guid).subscribe(response => {
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
    this.ruleService.updateRuleStatus(guid, status).subscribe(response => {
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
    this.ruleService.getRuleList(this.searchText).subscribe(response => {
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

}
