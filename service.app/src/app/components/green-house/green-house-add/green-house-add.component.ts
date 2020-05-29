import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { AppConstant, MessageAlertDataModel, DeleteAlertDataModel } from '../../../app.constants';
import { MatDialog } from '@angular/material';
import { GreenHouseService, NotificationService, Notification } from '../../../services';
import { MessageDialogComponent, DeleteDialogComponent } from '../..';
export interface DeviceTypeList {
  id: number;
  type: string;
}

@Component({
  selector: 'app-green-house-add',
  templateUrl: './green-house-add.component.html',
  styleUrls: ['./green-house-add.component.css']
})
export class GreenHouseAddComponent implements OnInit {
  @ViewChild('myFile', { static: false }) myFile: ElementRef;
  validstatus = false;
  handleImgInput = false;
  fileUrl: any;
  fileName = '';
  currentImage = '';
  fileToUpload: any;
  MessageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;
  status;
  moduleName = "Add Green House";
  greenhouseObject: any = {};
  greenHouseGuid = '';
  isEdit = false;
  hasImage = false;
  greenHouseForm: FormGroup;
  checkSubmitStatus = false;
  countryList = [];
  stateList = [];
  buttonname = 'SUBMIT';
  mediaUrl: any;
  arrystatus = [{ "name": "Active", "value": true }, { "name": "Inactive", "value": false }]
  constructor(
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public _appConstant: AppConstant,
    public dialog: MatDialog,
    public greenHouseService: GreenHouseService
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.greenHouseGuid != 'add') {
        this.getGreenhouseDetails(params.greenHouseGuid);
        this.greenHouseGuid = params.greenHouseGuid;
        this.moduleName = "Edit Green House";
        this.isEdit = true;
        this.buttonname = 'UPDATE'
      } else {
        this.greenhouseObject = { name: '', zipcode: '', countryGuid: '', stateGuid: '', isactive: 'true', city: '', latitude: '', longitude: '' }
      }
    });
  }

  ngOnInit() {
    this.mediaUrl = this._notificationService.apiBaseUrl
    this.getcountryList();

  }

  // imageRemove() {
  //   if (this.isEdit && this.hasImage) {
  //     this.spinner.show();
  //     this.greenHouseService.removeGHImage(this.greenHouseGuid).subscribe(response => {
  //       this.spinner.hide();
  //       if (response.isSuccess === true) {
  //         this.greenhouseObject['image'] = null;
  //         this.greenHouseForm.get('imageFile').setValue(null);
  //         this.fileToUpload = false;
  //       } else {
  //         this._notificationService.add(new Notification('error', response.message));
  //       }
  //     })
  //   } else {
  //     this.greenhouseObject['image'] = null;
  //     this.greenHouseForm.get('imageFile').setValue(null);
  //     this.fileToUpload = false;
  //   }

  // }

  imageRemove() {
    this.myFile.nativeElement.value = "";
    if (this.greenhouseObject['image'] == this.currentImage) {

      this.greenHouseForm.get('imageFile').setValue('');
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
        this.greenhouseObject['image'] = this.currentImage;
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
      else {
        this.spinner.hide();
        this.greenhouseObject['image'] = null;
        this.greenHouseForm.get('imageFile').setValue('');
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
    }

  }

  deleteImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Green House Image"),
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
        this.deletebuildingImg();
      }
    });
  }

  deletebuildingImg() {
    this.spinner.show();
    this.greenHouseService.removeGHImage(this.greenHouseGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.greenhouseObject['image'] = null;
        this.greenHouseForm.get('imageFile').setValue(null);
        this.fileToUpload = false;
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }


  createFormGroup() {
    this.greenHouseForm = new FormGroup({
      parentEntityGuid: new FormControl(''),
      countryGuid: new FormControl(null, [Validators.required]),
      stateGuid: new FormControl(null, [Validators.required]),
      city: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      zipcode: new FormControl('', [Validators.required, Validators.pattern('^[A-Z0-9 _]*$'), Validators.maxLength(7)]),
      description: new FormControl(''),
      address: new FormControl('', [Validators.required, Validators.maxLength(250)]),
      isactive: new FormControl('', [Validators.required]),
      guid: new FormControl(null),
      latitude: new FormControl('', [Validators.required, Validators.pattern('^(\\+|-)?(?:90(?:(?:\\.0{1,6})?)|(?:[0-9]|[1-8][0-9])(?:(?:\\.[0-9]{1,6})?))$')]),
      longitude: new FormControl('', [Validators.required, Validators.pattern('^(\\+|-)?(?:180(?:(?:\\.0{1,6})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\\.[0-9]{1,6})?))$')]),
      imageFile: new FormControl(''),
    });
  }

  /**
   * Add update Green house
   * */
  addGreenHouse() {

    this.checkSubmitStatus = true;

    if (this.isEdit) {
      this.greenHouseForm.get('guid').setValue(this.greenHouseGuid);
      this.greenHouseForm.get('isactive').setValue(this.greenhouseObject['isactive']);
    } else {
      this.greenHouseForm.get('isactive').setValue(true);
    }
    if (this.greenHouseForm.status === "VALID") {
      if (this.validstatus == true || !this.greenHouseForm.value.imageFile) {
        if (this.fileToUpload) {
          this.greenHouseForm.get('imageFile').setValue(this.fileToUpload);
        }
        this.spinner.show();
        let currentUser = JSON.parse(localStorage.getItem('currentUser'));
        this.greenHouseForm.get('parentEntityGuid').setValue(currentUser.userDetail.entityGuid);
        this.greenHouseService.addGreenhouse(this.greenHouseForm.value).subscribe(response => {
          this.spinner.hide();
          if (response.isSuccess === true) {
            if (this.isEdit) {
              this._notificationService.add(new Notification('success', "Greenhouse has been updated successfully."));
            } else {
              this._notificationService.add(new Notification('success', "Greenhouse has been added successfully."));
            }
            this.router.navigate(['/green-houses']);
          } else {
            this._notificationService.add(new Notification('error', response.message));
          }
        });
      } else {
        this.MessageAlertDataModel = {
          title: "Green House Image",
          message: "Invalid Image Type.",
          message2: "Upload .jpg, .jpeg, .png Image Only.",
          okButtonName: "OK",
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.MessageAlertDataModel,
          disableClose: false
        });
      }
    }
  }

  /**
   * Removefile type
   * @param type
   */
  removeFile(type) {
    if (type === 'image') {
      this.fileUrl = '';
      //this.floor_image_Ref.nativeElement.value = '';
    }
  }

  /**
   * Handle image input type
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
            that.greenhouseObject.image = this.fileUrl;
          }
        }
      } else {
        this.imageRemove();
        this.MessageAlertDataModel = {
          title: "Green House Image",
          message: "Invalid Image Type.",
          message2: "Upload .jpg, .jpeg, .png Image Only.",
          okButtonName: "OK",
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.MessageAlertDataModel,
          disableClose: false
        });
      }
    }
  }

  /**
   * Get green house details by greenhouseGuid
   * @param greenhouseGuid
   */
  getGreenhouseDetails(greenhouseGuid) {
    this.spinner.show();
    this.greenHouseService.getgreenhouseDetails(greenhouseGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.greenhouseObject = response.data;
        console.log(this.greenhouseObject);
        if (this.greenhouseObject.image) {
          this.greenhouseObject.image = this.mediaUrl + this.greenhouseObject.image;
          this.currentImage = this.greenhouseObject.image;
          this.hasImage = true;
        } else {
          this.hasImage = false;
        }
        this.greenHouseService.getstateList(response.data.countryGuid).subscribe(response => {
          this.spinner.hide();
          this.stateList = response.data;
        });
      }
    });
  }

  /**
   * Get country list
   * */
  getcountryList() {
    this.spinner.show();
    this.greenHouseService.getcountryList().subscribe(response => {
      this.spinner.hide();
      this.countryList = response.data;
    });
  }

  /**
   * 
   * @param event
   */
  changeCountry(event) {
    this.stateList = [];
    this.greenHouseForm.controls['stateGuid'].setValue(null, { emitEvent: true })
    if (event) {
      let id = event.value;
      this.spinner.show();
      this.greenHouseService.getstateList(id).subscribe(response => {
        this.spinner.hide();
        this.stateList = response.data;
      });
    }
  }

  getdata(val) {
    return val = val.toLowerCase();
  }

}
