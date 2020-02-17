import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { GreenHouseService, CropService, GatewayService, DeviceService, DashboardService } from '../../../services';
import { Notification, NotificationService } from 'app/services';
import { ActivatedRoute } from '@angular/router';
import { AppConstant, DeleteAlertDataModel } from '../../../app.constants';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";
import { MatDialog } from '@angular/material';

@Component({
  selector: 'app-green-house-details',
  templateUrl: './green-house-details.component.html',
  styleUrls: ['./green-house-details.component.css']
})
export class GreenHouseDetailsComponent implements OnInit {

  name: any;
  description: any;
  address1: any;
  address2: any;
  city: any;
  zipcode: any;
  status: any;
  image: any;
  greenHouseGuid: any;
  totalRecords = 0;
  cropList = [];
  deviceList = [];
  cropObject = {};
  cropForm: FormGroup;
  moduleName = "";
  buttonname = 'SUBMIT';
  checkSubmitStatus = false;
  isManageCrop = false;
  isEdit = false;
  cropGuid: any;
  deleteAlertDataModel: DeleteAlertDataModel;
  searchParameters = {
    pageNumber: 0,
    pageSize: -1,
    searchText: '',
    sortBy: 'name asc'
  };
  energyUsage: any;
  humidity: any;
  moisture: any;
  temperature: any;
  totalDevices: any;
  waterUsage: any;
  fileName: any;
  fileToUpload: any;
  fileUrl: any;

  public respondShow: boolean = false;
  Respond() {
    this.respondShow = !this.respondShow;
    this.refresh();
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public servive: GreenHouseService,
    public dialog: MatDialog,
    public cropService: CropService,
    private gatewayService: GatewayService,
    private deviceService: DeviceService,
    public _appConstant: AppConstant,
    public dashboardService: DashboardService,
    private _notificationService: NotificationService
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.greenHouseGuid) {
        this.greenHouseGuid = params.greenHouseGuid
      }

    })
  }

  ngOnInit() {
    this.getGreenHouseDetails(this.greenHouseGuid)
    this.getCropList(this.greenHouseGuid)
    this.getGatewayList(this.greenHouseGuid);
    this.getgreenHouse(this.greenHouseGuid);
  }

  createFormGroup() {
    this.cropForm = new FormGroup({
      greenHouseGuid: new FormControl(null),
      name: new FormControl('', [Validators.required]),
      isactive: new FormControl(''),
      imageFile: new FormControl(''),
    });
  }

  getGreenHouseDetails(greenHouseGuid) {
    this.spinner.show();
    this.servive.getgreenhouseDetails(greenHouseGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {

        this.name = response.data.name;
        this.description = response.data.description;
        this.address1 = response.data.address;
        this.address2 = response.data.address2;
        this.city = response.data.city;
        this.zipcode = response.data.zipcode;
        this.status = response.data.isactive;
        this.image = response.data.image;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  getCropList(greenHouseGuid) {
    this.spinner.show();
    this.cropService.getCrop(greenHouseGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.cropList = response.data;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.cropList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  getGatewayList(greenHouseGuid) {
    this.spinner.show();
    this.deviceService.getDeviceDetailsList(greenHouseGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.deviceList = response.data.items;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.deviceList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  getCropDetails(cropGuid) {
    this.moduleName = "Edit Crop";
    this.cropGuid = cropGuid;
    this.isEdit = true;
    this.buttonname = 'UPDATE';
    this.respondShow = true;
    this.spinner.show();
    this.cropService.getCropDetails(cropGuid).subscribe(response => {
      this.spinner.hide();
      this.cropObject = response.data;
      if (response.isSuccess === true) {
        this.cropObject = response.data;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.cropList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  manageCrop() {
    this.checkSubmitStatus = true;
    var data = {
      "greenHouseGuid": this.greenHouseGuid,
      "name": this.cropForm.value.name,
      "isactive": true,
      
    }
    if (this.isEdit) {
      if (this.cropGuid) {
        data["guid"] = this.cropGuid;
      }
      if (this.fileToUpload) {
        data["imageFile"] = this.fileToUpload;
      }
      data.isactive = this.cropObject['isactive']
    }
    else {
      data["imageFile"] = this.fileToUpload;
      this.cropForm.get('isactive').setValue(true);

    }
    if (this.cropForm.status === "VALID") {
      this.spinner.show();
      this.cropService.manageCrop(data).subscribe(response => {
        this.spinner.hide();
        this.respondShow = false;
        this.getCropList(this.greenHouseGuid);
        //if (response.isSuccess === true) {
        //  if (this.isEdit) {
        //    this._notificationService.add(new Notification('success', "Crop has been updated successfully."));
        //  } else {
        //    this._notificationService.add(new Notification('success', "Crop has been added successfully."));
        //  }
        //  this.isManageCrop = false;
        //} else {
        //  this._notificationService.add(new Notification('error', response.message));
        //}
      })
    }
  }

  refresh() {
    this.cropForm.reset();
    this.moduleName = "Add Crop";
    this.cropGuid = null;
    this.isEdit = false;
    this.buttonname = 'ADD';
    this.respondShow = true;
  }

  deleteModel(cropModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Crop",
      message: this._appConstant.msgConfirm.replace('modulename', "Crop"),
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
        this.deleteCrop(cropModel.guid);
      }
    });
  }

  deleteCrop(guid) {
    this.spinner.show();
    this.cropService.deleteCrop(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Crop")));
        this.getCropList(this.greenHouseGuid);
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  activeInactiveDevice(id: string, isActive: boolean, name: string) {
    var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: name,
      modulename: "Device"
    };
    this.deleteAlertDataModel = {
      title: "Status",
      message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname/gi, function (matched) {
        return mapObj[matched];
      }),
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
        this.changeGatewayStatus(id, isActive);

      }
    });

  }

  changeGatewayStatus(gatewayId, isActive) {
    this.spinner.show();
    this.gatewayService.changeStatus(gatewayId, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Device")));
        this.getGatewayList(this.greenHouseGuid);
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }

    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  getgreenHouse(greenhouseguid) {

    this.spinner.show();
    this.dashboardService.getGreenHouseDetail(greenhouseguid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.energyUsage = response.data.energyUsage
        this.humidity = response.data.humidity
        this.moisture = response.data.moisture
        this.temperature = response.data.temperature
        this.totalDevices = response.data.totalDevices
        this.waterUsage = response.data.waterUsage
        this.dashboardService.getSoilnutrition(greenhouseguid).subscribe(response => {
        })
      }
      else {
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
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
}
