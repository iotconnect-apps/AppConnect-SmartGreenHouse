import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { DeviceService, NotificationService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant, MessageAlertDataModel, DeleteAlertDataModel } from "../../../../app.constants";
import { Guid } from "guid-typescript";
import { hadwareobj } from './hardware-kit-model';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { upperCase, disable } from '@rxweb/reactive-form-validators';
import { MatDialog } from '@angular/material';
import { DeleteDialogComponent } from '../../..';
import { MessageDialogComponent } from '../../../common/message-dialog/message-dialog.component';

export interface DeviceTypeList {
  id: number;
  type: string;
}
export interface deviceobj {
  deviceid: any;
}
@Component({
  selector: 'app-hardware-add',
  templateUrl: './hardware-kit-add.component.html',
  styleUrls: ['./hardware-kit-add.component.css']
})


export class HardwareAddComponent implements OnInit {
  hardwareKitsarry = []
  show = false;
  hardwareobject = new hadwareobj();
  IsForUpdate = false;
  kitDevices = [];
  parentdevicearry = [];
  deviceobj = {};
  currentUser: any;
  fileUrl: any;
  fileName = '';
  fileToUpload: any = null;
  status;
  moduleName = "Add Hardware Kit";
  deviceObject = {};
  hardwareGuid = '';
  disabledtext = false;
  isEdit = false;
  deviceForm: FormGroup;
  checkSubmitStatus = false;
  buildingList = [];
  selectedBuilding = '';
  floorList = [];
  spaceList = [];
  templateList = [];
  dataSource: any = [];
  tagList: [];
  messageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;
  //displayedColumns: string[] = ['Device ID','Device Name','Tag','action'];
  displayedColumns: string[] = ['uniqueId', 'name', 'tag', 'parentUniqueId', 'action'];
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  index: any = -1;
  parentDeviceGuids: string;
  kitCodedata: any;
  constructor(
    public dialog: MatDialog,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private deviceService: DeviceService,
    public _appConstant: AppConstant
  ) {

    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.activatedRoute.params.subscribe(params => {
      if (params.hardwarekitGuid != null) {
        this.getDeviceDetails(params.hardwarekitGuid);
        this.hardwareGuid = params.hardwarekitGuid;
        this.moduleName = "Edit Hardware Kit";
        this.isEdit = true;
      } else {
        this.deviceObject = {}
      }
    });
    this.createFormGroup();
  }

  ngOnInit() {
    this.gettemplateLookup();
  }

  createFormGroup() {
    this.deviceForm = new FormGroup({
      kitTypeGuid: new FormControl('', [Validators.required]),
      uniqueId: new FormControl(''),
      name: new FormControl(''),
      type: new FormControl(0),
      deviceId: new FormControl({ value: '', disabled: this.disabledtext }),
      devicename: new FormControl(''),
      tag: new FormControl(''),
      companyGuid: new FormControl(null),
      greenHouseGuid: new FormControl(''),
      templateGuid: new FormControl(Guid.EMPTY),
      isProvisioned: new FormControl(false),
      isActive: new FormControl(true),
      //kitCode: new FormControl('',[Validators.required])
      kitCode: new FormControl({ value: '', disabled: this.isEdit }, [Validators.required])
    });
  }

  gettemplateLookup() {
    this.deviceService.getkittypes().subscribe(response => {
      this.templateList = response['data'];
    });
  }

  addHardwarekit() {
    this.checkSubmitStatus = true;
    if (this.deviceForm.status === "VALID") {
      this.spinner.show();
      this.deviceForm.patchValue({ 'companyGuid': this.currentUser.userDetail.companyId });
      (this.deviceForm.value.note == "" || typeof this.deviceForm.value.note === "undefined")
        ? this.deviceForm.patchValue({ 'note': "" })
        : "";

      let successMessage = this._appConstant.msgCreated.replace("modulename", "Hardware kit");

      if (this.isEdit) {
        successMessage = this._appConstant.msgUpdated.replace("modulename", "Hardware kit");
      }
      if (this.deviceForm.value.kitCode == undefined) {
        this.kitCodedata = this.kitCodedata;
      } else {
        this.kitCodedata = this.deviceForm.value.kitCode;
      }
      this.hardwareKitsarry.push({ "kitCode": this.kitCodedata, "kitDevices": this.kitDevices })
      var data = {
        "kitTypeGuid": this.deviceForm.value.kitTypeGuid,
        //"kitCode": this.deviceForm.value.kitCode,
        "hardwareKits": this.hardwareKitsarry
      }
      if (this.hardwareGuid) {
        data["guid"] = this.hardwareGuid;
      }
      this.deviceService.addUpdateHardwarekit(data, this.isEdit).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/admin/viewhardwarekit']);
          this._notificationService.add(new Notification('success', successMessage));
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      });

    }
  }

  removeFile(type) {
    if (type === 'image') {
      this.fileUrl = '';
    }
  }

  handleImageInput(event) {
    let files = event.target.files;
    if (files.length) {
      let fileType = files.item(0).name.split('.');
      let imagesTypes = ['jpeg', 'JPEG', 'jpg', 'JPG', 'png', 'PNG'];
      if (imagesTypes.indexOf(fileType[fileType.length - 1]) !== -1) {
        this.fileName = files.item(0).name;
        this.fileToUpload = files.item(0);
      } else {
        this.fileToUpload = null;
        this.fileName = '';
      }
    }

    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]);
      reader.onload = (innerEvent: any) => {
        this.fileUrl = innerEvent.target.result;
      }
    }
  }

  getDeviceDetails(hardwareGuid) {
    this.deviceService.getHardwarkitDetails(hardwareGuid).subscribe(response => {
      if (response.data) {
        this.kitCodedata = response.data.kitCode;
        let kitTypeGuid = response.data.kitTypeGuid.toUpperCase();
        response.data.kitTypeGuid = kitTypeGuid;
        this.deviceObject = response.data;
        this.kitDevices = response.data.kitDevices;
        this.dataSource = new MatTableDataSource(this.kitDevices);
        this.dataSource.paginator = this.paginator;
        if (response.data.kitDevices.length > 0) {
          for (var i = 0; i < response.data.kitDevices.length; i++) {
            if (response.data.kitDevices[i].parentUniqueId == null) {
              this.parentdevicearry.push({
                uniqueId: response.data.kitDevices[i].uniqueId,
                name: response.data.kitDevices[i].name
              })
            }
          }
        }
      }
    });
  }

  clickAdd() {
    if (this.hardwareobject.parentUniqueId == undefined || this.hardwareobject.parentUniqueId == '') {
      this.parentDeviceGuids = "";
      var objdata = {
        uniqueId: this.hardwareobject.uniqueId,
        name: this.hardwareobject.name
      }
      this.parentdevicearry.push(objdata);
    } else {
      for (var i = 0; i < this.parentdevicearry.length; i++) {
        if (this.hardwareobject.uniqueId == this.parentdevicearry[i].uniqueId) {
          this.parentdevicearry.splice(i, 1);
        }
      }
      this.parentDeviceGuids = this.hardwareobject.parentUniqueId;
    }
    if (this.hardwareobject.uniqueId == '' || this.hardwareobject.name == '' || this.hardwareobject.uniqueId == undefined || this.hardwareobject.name == undefined) {
      this.show = true;
    } else {
      this.show = false;
      if (this.index == -1) {
        var obj = {
          //guid: "00000000-0000-0000-0000-000000000000",
          //kitGuid: "00000000-0000-0000-0000-000000000000",
          //KitCode:this.deviceForm.value.kitCode,
          parentUniqueId: this.parentDeviceGuids,
          uniqueId: this.hardwareobject.uniqueId,
          name: this.hardwareobject.name,
          tag: this.hardwareobject.tag,
          note: "test note",
          //isProvisioned:true
        }

        this.kitDevices.push(obj);
        this.hardwareobject = new hadwareobj();

      } else {
        var obj = {
          //guid: "00000000-0000-0000-0000-000000000000",
          //kitGuid: "00000000-0000-0000-0000-000000000000",
          //KitCode:this.deviceForm.value.kitCode,
          parentUniqueId: this.parentDeviceGuids,
          uniqueId: this.hardwareobject.uniqueId,
          name: this.hardwareobject.name,
          tag: this.hardwareobject.tag,
          note: "test note",
          //isProvisioned:true
        }
        this.kitDevices.splice(this.index, 1, obj);
      }
      this.hardwareobject = new hadwareobj();
      this.index = -1
    }
    this.dataSource = new MatTableDataSource(this.kitDevices);
    this.dataSource.paginator = this.paginator;
  }
  DeleteItem(i) {
    var hasChild = false;
    var isPArent = false;
    for (var j = 0; j < (this.dataSource._data._value).length; j++) {
      if (!this.dataSource._data._value[i].parentUniqueId) {
        isPArent = true;
      }
      if (this.dataSource._data._value[j].parentUniqueId) {
        hasChild = true;
      }
    }

    

    if (isPArent && hasChild) {
      this.messageAlertDataModel = {
        title: "Message",
        message: this._appConstant.msgWarning,
        okButtonName: "Ok"
      };
      const dialogRef = this.dialog.open(MessageDialogComponent, {
        width: '400px',
        height: 'auto',
        data: this.messageAlertDataModel,
        disableClose: false
      });
    }
    else if (isPArent && !hasChild) {
      this.deleteAlertDataModel = {
        title: "Delete Device",
        message: this._appConstant.msgConfirm.replace('modulename', "Hardware kit"),
        okButtonName: "Confirm",
        cancelButtonName: "Cancel",
      };
      const dialogRef = this.dialog.open(DeleteDialogComponent, {
        width: '400px',
        height: 'auto',
        data: this.deleteAlertDataModel,
        disableClose: false
      });
      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.hardwareobject = this.kitDevices[i];
          this.kitDevices.splice(i, 1);
          this.dataSource = new MatTableDataSource(this.kitDevices);
          this.dataSource.paginator = this.paginator;
          this.parentdevicearry.splice(i, 1);
        }
      });
    }
    else {
      this.deleteAlertDataModel = {
        title: "Delete Device",
        message: this._appConstant.msgConfirm.replace('modulename', "Hardware kit"),
        okButtonName: "Confirm",
        cancelButtonName: "Cancel",
      };
      const dialogRef = this.dialog.open(DeleteDialogComponent, {
        width: '400px',
        height: 'auto',
        data: this.deleteAlertDataModel,
        disableClose: false
      });
      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.kitDevices.splice(i, 1);
          this.dataSource = new MatTableDataSource(this.kitDevices);
          this.dataSource.paginator = this.paginator;
        }
      });
    }

  }


  EditItem(i) {
    this.deviceForm.get('deviceId').disable()
    this.hardwareobject = this.kitDevices[i];
    this.index = i;
    this.IsForUpdate = true;
  }

  getTag(event) {
    let id = event.value;
    if (id) {
      this.spinner.show();
      this.deviceService.getTagLookup(id).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.tagList = response.data;
        }
        else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      });
    }
  }

}
