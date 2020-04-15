import { Component, OnInit, ViewChild } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { GreenHouseService, CropService, GatewayService, DeviceService, DashboardService } from '../../../services';
import { Notification, NotificationService } from 'app/services';
import { ActivatedRoute } from '@angular/router';
import { AppConstant, DeleteAlertDataModel } from '../../../app.constants';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";
import { MatDialog } from '@angular/material';
import { GoogleChartComponent } from 'ng2-google-charts'

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
  mediaUrl: any;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));

  @ViewChild('cchart', { static: false }) cchart: GoogleChartComponent;
  columnArray: any = [];
  headFormate: any = {
    columns: this.columnArray,
    type: 'NumberFormat'
  };
  bgColor = '#fff';
  chartHeight = 300;
  chartWidth = '100%';

  chart = {
    'waterConsumption': {
      chartType: 'ColumnChart',
      dataTable: [],
      options: {
        height: this.chartHeight,
        width: this.chartWidth,
        interpolateNulls: true,
        backgroundColor: this.bgColor,
        hAxis: {
          title: 'Date/Time',
          gridlines: {
            count: 5
          },
        },
        vAxis: {
          title: 'Values',
          gridlines: {
            count: 1
          },
        }
      },
      formatters: this.headFormate
    },
    'energyConsumption': {
      chartType: 'ColumnChart',
      dataTable: [],
      options: {
        height: this.chartHeight,
        width: this.chartWidth,
        interpolateNulls: true,
        backgroundColor: this.bgColor,
        hAxis: {
          title: 'Date/Time',
          gridlines: {
            count: 5
          },
        },
        vAxis: {
          title: 'Values',
          gridlines: {
            count: 1
          },
        }
      },
      formatters: this.headFormate
    },
    'soilNutritions': {
      chartType: 'ColumnChart',
      dataTable: [],
      options: {
        height: this.chartHeight,
        width: this.chartWidth,
        interpolateNulls: true,
        backgroundColor: this.bgColor,
        hAxis: {
          title: 'Date/Time',
          gridlines: {
            count: 5
          },
        },
        vAxis: {
          title: 'Values',
          gridlines: {
            count: 1
          },
        }
      },
      formatters: this.headFormate
    }
  };

  public respondShow: boolean = false;

  Respond() {
    this.cropForm.reset();
    this.respondShow = !this.respondShow;
    this.refresh();
  }

  closerepond() {
    this.checkSubmitStatus = false;
    this.respondShow = false;
    this.cropForm.reset();
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
    this.mediaUrl = this._notificationService.apiBaseUrl
    this.getGreenHouseDetails(this.greenHouseGuid)
    this.getCropList(this.greenHouseGuid)
    this.getKitList(this.greenHouseGuid);
    this.getgreenHouse(this.greenHouseGuid);
    this.getEnergyUsageChartData();
    this.getSoilnutritionChartData();
    this.getWaterConsumptionChartData();
  }

  /**
   * Get data for water consumpation chart
   * */
  getWaterConsumptionChartData() {
    let obj = { companyGuid: this.currentUser.userDetail.companyId, greenHouseGuid: this.greenHouseGuid };
    let data = []
    this.dashboardService.getWaterUsageChartData(obj).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.length) {
          data.push(['Months', 'Water Consumption']);
        }
        response.data.forEach(element => {
          data.push([element.month, parseInt(element.value)]);
        });
        this.createChart('waterConsumption', data, 'Months', 'gal');
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });


  }

  /**
   * Get data for Energy usage chart
   * */
  getEnergyUsageChartData() {
    let obj = { companyGuid: this.currentUser.userDetail.companyId, greenHouseGuid: this.greenHouseGuid };
    let data = []
    this.dashboardService.getEnergyUsageChartData(obj).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.length) {
          data.push(['Months', 'Energy']);
        }
        response.data.forEach(element => {
          data.push([element.month, parseInt(element.value)]);
        });
        this.createChart('energyConsumption', data, 'Months', 'KWH');
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });


  }

  /**
   * Get data for soil nutrition chart
   * */
  getSoilnutritionChartData() {
    let obj = { companyGuid: this.currentUser.userDetail.companyId, greenHouseGuid: this.greenHouseGuid };
    let data = []
    this.dashboardService.getSoilnutritionChartData(obj).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.length) {
          data.push(['pH Level', 'N', 'P', 'K']);
        }
        response.data.forEach(element => {
          data.push([element.phLevel, parseInt(element['n']), parseInt(element['p']), parseInt(element['k'])]);
        });
        this.createChart('soilNutritions', data, 'pH Level', '% Availability');
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });


  }

  /**
   * Create chart
   * @param key
   * @param data
   * @param hAxisTitle
   * @param vAxisTitle
   */
  createChart(key, data, hAxisTitle, vAxisTitle) {
    this.chart[key] = {
      chartType: 'ColumnChart',
      dataTable: data,
      options: {
        height: this.chartHeight,
        width: this.chartWidth,
        interpolateNulls: true,
        backgroundColor: this.bgColor,
        hAxis: {
          title: hAxisTitle,
          gridlines: {
            count: 5
          },
        },
        vAxis: {
          title: vAxisTitle,
          gridlines: {
            count: 1
          },
        }
      },
      formatters: this.headFormate
    };
  }

  /**
   * Create form cropForm
   * */
  createFormGroup() {
    this.cropForm = new FormGroup({
      greenHouseGuid: new FormControl(null),
      name: new FormControl('', [Validators.required]),
      isactive: new FormControl(''),
      imageFile: new FormControl(''),
    });
  }

  /**
   * Get green house details by greenHouseGuid
   * @param greenHouseGuid
   */
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

  /**
   * Get crop list by greenHouseGuid
   * @param greenHouseGuid
   */
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

  /**
   * Get H/W kit list by greenHouseGuid
   * @param greenHouseGuid
   */
  getKitList(greenHouseGuid) {
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

  /**
   * Get Crop details by cropGuid
   * @param cropGuid
   */
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

  /**
   * Add update crop
   * */
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

  /**
   * Refresh the form
   * */
  refresh() {
    this.createFormGroup();
    this.cropForm.reset();
    this.moduleName = "Add Crop";
    this.cropGuid = null;
    this.isEdit = false;
    this.buttonname = 'ADD';
    this.respondShow = true;
    this.checkSubmitStatus = false;
  }

  /**
   * Delete model confirmation popup
   * @param cropModel
   */
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

  /**
   * Delete crop by crop guid
   * @param guid
   */
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

  /**
   * Active inactive device confirmation popup
   * @param id
   * @param isActive
   * @param name
   */
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
        this.changeKitStatus(id, isActive);

      }
    });

  }

  /**
   * Change H/W kit status by kitId
   * @param kitId
   * @param isActive
   */
  changeKitStatus(kitId, isActive) {
    this.spinner.show();
    this.gatewayService.changeStatus(kitId, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Device")));
        this.getKitList(this.greenHouseGuid);
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }

    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Get green house details and soil nutrition data by greenhouseguid
   * @param greenhouseguid
   */
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
      }
      else {
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Handle input image
   * @param event
   */
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
