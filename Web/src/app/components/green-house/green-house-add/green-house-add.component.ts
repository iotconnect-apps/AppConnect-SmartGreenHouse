import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { GreenHouseService } from 'app/services/green-house/green-house.service';
import { Notification, NotificationService } from 'app/services';
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

  fileUrl: any;
  fileName = '';
  fileToUpload: any;
  status;
  moduleName = "Add Green House";
  greenhouseObject = {};
  greenHouseGuid = '';
  isEdit = false;
  greenHouseForm: FormGroup;
  checkSubmitStatus = false;
  countryList = [];
  cityList = [];
  buttonname = 'SUBMIT'
  arrystatus = [{ "name": "Active", "value": true }, { "name": "Inactive", "value": false }]
  constructor(
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
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
    this.getcountryList();

  }

  createFormGroup() {
    this.greenHouseForm = new FormGroup({
      parentEntityGuid: new FormControl(''),
      countryGuid: new FormControl('', [Validators.required]),
      stateGuid: new FormControl('', [Validators.required]),
      city: new FormControl('', [Validators.required]),
      name: new FormControl(''),
      zipcode: new FormControl('', [Validators.pattern('^[0-9]*$')]),
      description: new FormControl(''),
      address: new FormControl('', [Validators.required]),
      isactive: new FormControl('', [Validators.required]),
      guid: new FormControl(null),
      latitude: new FormControl('', [Validators.required]),
      longitude: new FormControl('', [Validators.required]),
      imageFile: new FormControl(''),
    });
  }

  addGreenHouse() {
    
    this.checkSubmitStatus = true;

    if (this.isEdit) {
      this.greenHouseForm.get('guid').setValue(this.greenHouseGuid);
      this.greenHouseForm.get('isactive').setValue(this.greenhouseObject['isactive']);
    } else {
      this.greenHouseForm.get('isactive').setValue(true);
    }
    if (this.greenHouseForm.status === "VALID") {
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
      })
    }
  }

  removeFile(type) {
    if (type === 'image') {
      this.fileUrl = '';
      //this.floor_image_Ref.nativeElement.value = '';
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

  getGreenhouseDetails(greenhouseGuid) {
    this.spinner.show();
    this.greenHouseService.getgreenhouseDetails(greenhouseGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.greenhouseObject = response.data;
        this.greenHouseService.getcitylist(response.data.countryGuid).subscribe(response => {
          this.spinner.hide();
          this.cityList = response.data;
        });
      }
    });
  }

  getcountryList() {
    this.spinner.show();
    this.greenHouseService.getcountryList().subscribe(response => {
      this.spinner.hide();
      this.countryList = response.data;
    });
  }

  changeCity(event) {
    let id = event.value;
    this.spinner.show();
    this.greenHouseService.getcitylist(id).subscribe(response => {
      this.spinner.hide();
      this.cityList = response.data;
    });
  }

  getdata(val) {
    return val = val.toLowerCase();
  }

}
