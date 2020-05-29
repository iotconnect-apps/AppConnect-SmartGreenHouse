import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { GreenHouseService, CropService, GatewayService, DeviceService, DashboardService } from '../../../services';
import { Notification, NotificationService } from 'app/services';
import { ActivatedRoute, Router } from '@angular/router';
import { AppConstant, DeleteAlertDataModel, MessageAlertDataModel } from '../../../app.constants';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";
import { MatDialog } from '@angular/material';
import { GoogleChartComponent } from 'ng2-google-charts'
import { MessageDialogComponent } from '../..';

@Component({
  selector: 'app-green-house-details',
  templateUrl: './green-house-details.component.html',
  styleUrls: ['./green-house-details.component.css']
})
export class GreenHouseDetailsComponent implements OnInit {
  @ViewChild('myFile', { static: false }) myFile: ElementRef;
  handleImgInput = false;
  validstatus = false;
  MesageAlertDataModel: MessageAlertDataModel;
  name: any;
  currentImage: any;
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
  cropObject: any = {};
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

  stats = {
    energyUsage: 0,
    humidity: 0,
    moisture: 0,
    temperature: 0,
    totalDevices: 0,
    waterUsage: 0
  }

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
  lastSyncDate = '';
  Respond() {
    this.cropForm.reset();
    this.currentImage = '';
    this.respondShow = !this.respondShow;
    this.refresh();
    this.fileToUpload = null;
    this.cropObject.image = '';
    this.fileName = '';
  }

  closerepond() {
    this.currentImage = '';
    this.fileToUpload = null;
    this.checkSubmitStatus = false;
    this.respondShow = false;
    this.cropForm.reset();
    this.cropObject.image = '';
    this.fileName = '';
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
    private _notificationService: NotificationService,
    private router: Router
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


  imageRemove() {
    this.myFile.nativeElement.value = "";
    if (this.cropObject['image'] == this.currentImage) {
      this.cropForm.get('imageFile').setValue('');
      if (!this.handleImgInput) {
        this.handleImgInput = false;
        this.deleteImgModel();
      }
      else {
        this.handleImgInput = false;
      }
    }
    else {
      if (this.currentImage) {
        this.spinner.hide();
        this.cropObject['image'] = this.currentImage;
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
      else {
        this.spinner.hide();
        this.cropObject['image'] = null;
        this.cropForm.get('imageFile').setValue('');
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
    }
  }

  deleteCropImg() {
    this.spinner.show();
    this.cropService.removeCropImage(this.cropObject.guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.currentImage = '';
        this.cropObject['image'] = null;
        this.cropForm.get('imageFile').setValue(null);
        this.fileToUpload = false;
        this.fileName = '';
        this.getCropList(this.greenHouseGuid);
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  deleteImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Crop Image"),
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
        this.deleteCropImg();
      }
    });
  }

  /**
   * Get data for water consumpation chart
   * */
  getWaterConsumptionChartData() {
    let obj = { companyGuid: this.currentUser.userDetail.companyId, greenHouseGuid: this.greenHouseGuid };
    let data = []
    this.dashboardService.getWaterUsageChartData(obj).subscribe(response => {
      //this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.length) {
          data.push(['Months', 'Water Consumption']);
        }
        response.data.forEach(element => {
          data.push([element.month, parseFloat(element.value)]);
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
      //this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.length) {
          data.push(['Months', 'Energy']);
        }
        response.data.forEach(element => {
          data.push([element.month, parseFloat(element.value)]);
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
      //this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.length) {
          data.push(['pH Level', 'N', 'P', 'K']);
        }
        response.data.forEach(element => {
          data.push([element.day, parseFloat(element['n']), parseFloat(element['p']), parseFloat(element['k'])]);
        });
        this.createChart('soilNutritions', data, 'Days', '% pH Level');
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
    let legend = { position: 'none' };
    var hAxis = {
      title: hAxisTitle,
      gridlines: {
        count: 5
      },
      slantedText: true,
      slantedTextAngle: 45,
    };
    if (key === 'soilNutritions') {
      legend = { position: 'right' };

    }
   if (key === 'energyConsumption') {
    this.chart[key] = {
      chartType: 'ColumnChart',
      dataTable: data,
      options: {
        height: this.chartHeight,
        width: this.chartWidth,
        interpolateNulls: true,
        legend: legend,
        backgroundColor: this.bgColor,
        colors: ['#ed734c'],
        hAxis: hAxis,
        vAxis: {
          title: vAxisTitle,
        }
      },
      formatters: this.headFormate
    };
    }else{
    this.chart[key] = {
      chartType: 'ColumnChart',
      dataTable: data,
      options: {
        height: this.chartHeight,
        width: this.chartWidth,
        interpolateNulls: true,
        legend: legend,
        backgroundColor: this.bgColor,
        hAxis: hAxis,
        vAxis: {
          title: vAxisTitle,
        }
      },
      formatters: this.headFormate
    };
    }
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
      //this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data) {

          this.name = response.data.name;
          this.description = response.data.description;
          this.address1 = response.data.address;
          this.address2 = response.data.address2;
          this.city = response.data.city;
          this.zipcode = response.data.zipcode;
          this.status = response.data.isactive;
          this.image = response.data.image;
        } else {
          this.router.navigate(['/green-houses']);
          this._notificationService.add(new Notification('error', 'Green house not found'));
        }
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
      //this.spinner.hide();
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
    this.fileName = '';
    this.currentImage = '';
    this.fileToUpload = false;
    this.cropObject.image = '';
    this.isEdit = true;
    this.buttonname = 'UPDATE';
    this.respondShow = true;
    this.spinner.show();
    this.cropService.getCropDetails(cropGuid).subscribe(response => {
      this.spinner.hide();
      this.cropObject = response.data;
      if (response.isSuccess === true) {
        this.cropObject = response.data;
        if (this.cropObject.image) {
          this.cropObject.image = this.mediaUrl + this.cropObject.image;
          this.currentImage = this.cropObject.image;
        }
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

        this.getCropList(this.greenHouseGuid);
        if (response.isSuccess === true) {
          this.respondShow = false;
          this.cropForm.reset();
          if (this.isEdit) {
            this._notificationService.add(new Notification('success', "Crop has been updated successfully."));
          } else {
            this._notificationService.add(new Notification('success', "Crop has been added successfully."));
          }
          this.isManageCrop = false;
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
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
        this.lastSyncDate = response.lastSyncDate;
        this.stats.energyUsage = (response.data.totalEnergyCount) ? response.data.totalEnergyCount : 0;
        this.stats.humidity = (response.data.avgHumidity) ? response.data.avgHumidity : 0;
        this.stats.moisture = (response.data.avgMoisture) ? response.data.avgMoisture : 0;
        this.stats.temperature = (response.data.avgTemp) ? response.data.avgTemp : 0;
        this.stats.totalDevices = (response.data.totalDevice) ? response.data.totalDevice : 0;
        this.stats.waterUsage = (response.data.totalWaterUsage) ? response.data.totalWaterUsage : 0;
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
    this.handleImgInput = true;
    let files = event.target.files;
    var that = this;
    if (files.length) {
      let fileType = files.item(0).name.split('.');
      let imagesTypes = ['jpeg', 'JPEG', 'jpg', 'JPG', 'png', 'PNG'];
      if (imagesTypes.indexOf(fileType[fileType.length - 1]) !== -1) {
        this.validstatus = true;
        this.fileName = files.item(0).name;
        this.fileToUpload = files.item(0);
        if (event.target.files && event.target.files[0]) {
          var reader = new FileReader();
          reader.readAsDataURL(event.target.files[0]);
          reader.onload = (innerEvent: any) => {
            this.fileUrl = innerEvent.target.result;
            that.cropObject.image = this.fileUrl
          }
        }
      } else {
        this.imageRemove();
        this.MesageAlertDataModel = {
          title: "Crop Image",
          message: "Invalid Image Type.",
          message2: "Upload .jpg, .jpeg, .png Image Only.",
          okButtonName: "OK",
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.MesageAlertDataModel,
          disableClose: false
        });
      }
    }
  }
}
