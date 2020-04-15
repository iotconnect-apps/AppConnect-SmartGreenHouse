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
  checkchildflag = false;
  emtycheck: any;
  Devices = [];
  hidediv = false;
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
  objForm: FormGroup;
  deviceForm: FormGroup;
  checkSubmitStatus = false;
  checkSubmitStatusinfrm = false;
  checkobjStatus = false;
  buildingList = [];
  selectedBuilding = '';
  floorList = [];
  spaceList = [];
  templateList = [];
  dataSource: any = [];
  tagList: [];
  messageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;
  displayedColumns: string[] = ['uniqueId', 'name', 'tag', 'parentUniqueId', 'message', 'action'];
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  index: any = -1;
  parentDeviceGuids: string;
  kitCodedata: any;
  uniquedataId: any;
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
        this.checkchildflag = true;
      } else {
        this.deviceObject = {}
      }
    });
    this.createFormGroup();
    this.createGroup();
  }

  ngOnInit() {
    this.emtycheck = this.kitDevices.length
    this.gettemplateLookup();
    this.hidediv = true
  }

  createFormGroup() {
    this.deviceForm = new FormGroup({
      kitTypeGuid: new FormControl('', [Validators.required]),
      uniqueId: new FormControl(''),
      name: new FormControl(''),
      type: new FormControl(0),
      companyGuid: new FormControl(null),
      templateGuid: new FormControl(Guid.EMPTY),
      isProvisioned: new FormControl(false),
      isActive: new FormControl(true),
      kitCode: new FormControl({ value: '', disabled: this.isEdit }, [Validators.required, Validators.pattern('^[A-Za-z0-9 ]+$')])
    });
  }
  createGroup() {
    this.objForm = new FormGroup({
      deviceId: new FormControl('', [Validators.required, Validators.pattern('^[A-Za-z0-9 ]+$')]),
      devicename: new FormControl('', [Validators.required]),
      tag: new FormControl(''),
      greenHouseGuid: new FormControl('')
    });
  }

  gettemplateLookup() {
    this.deviceService.getallkittypes().subscribe(response => {
      this.templateList = response['data'];
    });
  }

  addHardwarekit() {
    this.checkSubmitStatus = true;
    if (this.deviceForm.status === "VALID") {
      if (this.kitDevices.length > 0) {
        if (this.checkchildflag == true) {
          if (this.deviceForm.value.kitCode == undefined) {
            this.kitCodedata = this.kitCodedata;
          } else {
            this.kitCodedata = this.deviceForm.value.kitCode;
          }
          //this.hardwareKitsarry.push({ "kitCode": this.kitCodedata, "kitDevices": this.kitDevices })
          var data = {
            "kitTypeGuid": this.deviceForm.value.kitTypeGuid,
            "hardwareKits": [{ "kitCode": this.kitCodedata, "kitDevices": this.kitDevices }]
          }
          this.spinner.show();
          this.deviceForm.patchValue({ 'companyGuid': this.currentUser.userDetail.companyId });
          let successMessage = this._appConstant.msgCreated.replace("modulename", "Hardware kit");

          if (this.isEdit) {
            successMessage = this._appConstant.msgUpdated.replace("modulename", "Hardware kit");
          }
          this.deviceService.addUpdateHardwarekit(data, this.isEdit).subscribe(response => {
            this.spinner.hide();
            if (response.isSuccess === true) {
              this.router.navigate(['/admin/hardwarekits']);
              this._notificationService.add(new Notification('success', successMessage));
            } else {
              let vals = response.data.sort((a, b) => (a.uniqueId > b.uniqueId) ? 1 : -1)
              console.log("mainssss", vals)
              this.dataSource = new MatTableDataSource(response.data);
              this.dataSource.paginator = this.paginator;
              this._notificationService.add(new Notification('error', response.message));
            }
          }, error => {
            this.spinner.hide();
            this._notificationService.add(new Notification('error', error));
          });
        } else {
          this.messageAlertDataModel = {
            title: "Message",
            message: this._appConstant.msgHKchildRequired,
            okButtonName: "Ok"
          };
          const dialogRef = this.dialog.open(MessageDialogComponent, {
            width: '400px',
            height: 'auto',
            data: this.messageAlertDataModel,
            disableClose: false
          });
        }
      } else {
        this.messageAlertDataModel = {
          title: "Message",
          message: this._appConstant.msgHKdevicedRequired,
          okButtonName: "Ok"
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.messageAlertDataModel,
          disableClose: false
        });
      }
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
        this.getTag(kitTypeGuid)
        response.data.kitTypeGuid = kitTypeGuid;
        this.deviceObject = response.data;
        this.kitDevices = response.data.kitDevices;
        this.emtycheck = this.kitDevices.length
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
  addclickobj() {
    this.checkSubmitStatusinfrm = true;
    if (this.objForm.status === "VALID") {
      this.objForm.get('deviceId').enable()
      if (this.objForm.value.greenHouseGuid == undefined || this.objForm.value.greenHouseGuid == '') {
        var objdata = {
          uniqueId: this.objForm.value.deviceId,
          name: this.objForm.value.devicename
        }
        this.parentdevicearry.push(objdata);
      }
      if (this.index == -1) {
        /*if(this.objForm.value.greenHouseGuid){
          this.checkchildflag = true;
        }*/
        if (this.kitDevices.length > 0) {
          let objdata = this.kitDevices.find(o => o.uniqueId === this.objForm.value.deviceId);
          //console.log("datsss",objdata)
          if (objdata == undefined) {
            var obj = {
              parentUniqueId: this.objForm.value.greenHouseGuid,
              uniqueId: this.objForm.value.deviceId,
              name: this.objForm.value.devicename,
              tag: this.objForm.value.tag,
              note: ""
            }

            this.kitDevices.push(obj);
            this.hardwareobject = new hadwareobj();
          } else {
            this.messageAlertDataModel = {
              title: "Message",
              message: this._appConstant.msgHKuniqueId,
              okButtonName: "Ok"
            };
            const dialogRef = this.dialog.open(MessageDialogComponent, {
              width: '400px',
              height: 'auto',
              data: this.messageAlertDataModel,
              disableClose: false
            });
          }
        } else {
          var obj = {
            parentUniqueId: this.objForm.value.greenHouseGuid,
            uniqueId: this.objForm.value.deviceId,
            name: this.objForm.value.devicename,
            tag: this.objForm.value.tag,
            note: ""
          }

          this.kitDevices.push(obj);
          this.hardwareobject = new hadwareobj();
        }


      } else {
        for (var i = 0; i < this.parentdevicearry.length; i++) {
          if (this.kitDevices[this.index].uniqueId == this.parentdevicearry[i].uniqueId) {
            this.parentdevicearry.splice(i, 1);
          }
        }

        /*if(this.objForm.value.greenHouseGuid){
          this.checkchildflag = true;
        }*/
        if (this.IsForUpdate == true && this.objForm.value.tag == '') {
          this.objForm.value.greenHouseGuid = '';
          this.kitDevices[this.index].parentUniqueId = '';
        }
        if (this.hardwareGuid) {
          var unidata = this.uniquedataId;
        } else {
          var unidata = this.objForm.value.deviceId;
        }
        //let objdata = this.kitDevices.find(o => o.uniqueId === this.objForm.value.deviceId);
        //console.log("datsss",objdata)
        // if(objdata == undefined){
        var obj = {
          parentUniqueId: this.objForm.value.greenHouseGuid,
          uniqueId: unidata,
          name: this.objForm.value.devicename,
          tag: this.objForm.value.tag,
          note: ""
        }
        this.kitDevices.splice(this.index, 1, obj);
        /*} else{
          this.messageAlertDataModel = {
            title: "Message",
            message: this._appConstant.msgHKuniqueId,
            okButtonName: "Ok"
          };
          const dialogRef = this.dialog.open(MessageDialogComponent, {
            width: '400px',
            height: 'auto',
            data: this.messageAlertDataModel,
            disableClose: false
          });
        }*/
      }
      this.objForm.reset();
      this.objForm.get('deviceId').enable()
      this.objForm.get('tag').enable()
      this.checkSubmitStatusinfrm = false;
      this.index = -1
      this.hidediv = true;
      this.emtycheck = this.kitDevices.length
      this.dataSource = new MatTableDataSource(this.kitDevices);
      for (var j = 0; j < this.kitDevices.length; j++) {
        if (this.kitDevices[j].parentUniqueId) {
          this.checkchildflag = true;
        } else {
          this.checkchildflag = false;
        }
      }
      this.dataSource.paginator = this.paginator;

    }
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
          //this.hardwareobject = this.kitDevices[i];
          this.kitDevices.splice(i, 1);
          this.dataSource = new MatTableDataSource(this.kitDevices);
          this.emtycheck = this.kitDevices.length
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
    if (this.kitDevices[i].parentUniqueId == null || this.kitDevices[i].parentUniqueId == '') {
      this.objForm.get('tag').disable()
      this.hidediv = true;
    } else {
      if (this.kitDevices[i].tag) {
        this.hidediv = false;
      }

    }
    if (this.kitDevices[i].tag) {
      var tagdata = this.kitDevices[i].tag.toUpperCase()
    }
    if (this.hardwareGuid) {
      this.objForm.get('deviceId').disable()
    }
    this.objForm.get('deviceId').setValue(this.kitDevices[i].uniqueId);
    this.objForm.get('devicename').setValue(this.kitDevices[i].name);
    this.objForm.get('tag').setValue(tagdata);
    this.objForm.get('greenHouseGuid').setValue(this.kitDevices[i].parentUniqueId);
    this.uniquedataId = this.kitDevices[i].uniqueId;
    this.index = i;
    this.IsForUpdate = true;
  }

  getTag(id) {
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
  gettagname(tagid, tlist) {
    if (tlist && tagid != undefined && tagid != '' && tagid != '00000000-0000-0000-0000-000000000000') {
      let tagids = tagid.toUpperCase()
      let obj = tlist.find(o => o.value === tagids);
      return obj.text;
    }


  }
  getTagdata(val) {
    if (val == undefined || val == '') {
      if (this.IsForUpdate == true) {
        //this.kitDevices[this.index].parentUniqueId = '';
        this.hidediv = true
      } else {
        this.hidediv = true
      }

    } else {
      this.objForm.get('greenHouseGuid').setValue(this.parentdevicearry[0].uniqueId);
      //console.log("datsss",this.parentdevicearry[0].uniqueId)
      this.hidediv = false;
    }
  }

}
