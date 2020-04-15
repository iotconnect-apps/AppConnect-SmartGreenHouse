import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { GatewayService, NotificationService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant } from "../../../app.constants";

@Component({
  selector: 'app-gateway-add-device',
  templateUrl: './gateway-add-device.component.html',
  styleUrls: ['./gateway-add-device.component.css']
})
export class GatewayAddDeviceComponent implements OnInit {

  currentUser: any;
  fileUrl: any;
  fileName = '';
  fileToUpload: any = null;
  status;
  moduleName = "Add Device";
  gatewayObject = {};
  gatewayGuid = '';
  isEdit = false;
  gatewayForm: FormGroup;
  checkSubmitStatus = false;
  templateList = [];
  greenhouseList = [];


  constructor(
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private gatewayService: GatewayService,
    public _appConstant: AppConstant
  ) {
    this.createFormGroup();
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.activatedRoute.params.subscribe(params => {
      if (params.gatewayGuid != 'add') {
        this.getgatewayDetails(params.gatewayGuid);
        this.gatewayGuid = params.gatewayGuid;
        this.moduleName = "Edit Device";
        this.isEdit = true;
      } else {
        this.gatewayObject = {}
      }
    });
  }

  ngOnInit() {
    this.getTemplateLookup();
    this.getGreenHouseLookup();
  }
  createFormGroup() {
    this.gatewayForm = new FormGroup({
      uniqueId: new FormControl('', [Validators.required]),
      name: new FormControl('', [Validators.required]),
      templateGuid: new FormControl('', [Validators.required]),
      note: new FormControl(""),
      companyGuid: new FormControl(null),
      greenHouseGuid: new FormControl('', [Validators.required]),
      parentDeviceGuid: new FormControl(null),
      isProvisioned: new FormControl(false),
      isActive: new FormControl(true)
    });
  }


	/**
	 * Get template lookup
	 * only gateway supported template
	 */
  getTemplateLookup() {
    this.spinner.show();
    this.gatewayService.getTemplateLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.templateList = response['data'];
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

	/**
	 * 
	 * Get greenhouse lookup
	*/
  getGreenHouseLookup() {
    this.spinner.show();
    this.gatewayService.getGreenHouseLookup().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.greenhouseList = response['data'];
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  log(obj) {
    console.log(obj);
  }

	/**
	 * 
	 * Add/Update Gateway
	 * 
	*/
  addgateway() {
    this.checkSubmitStatus = true;
    // this.gatewayForm.patchValue({ 'parentDeviceGuid': null });
    // this.gatewayForm.patchValue({ 'isActive': true });
    // this.gatewayForm.patchValue({ 'isProvisioned': false });

    if (this.gatewayForm.status === "VALID") {
      this.spinner.show();
      // overwrite default values
      this.gatewayForm.patchValue({ 'companyGuid': this.currentUser.userDetail.companyId });
      (this.gatewayForm.value.note == "" || typeof this.gatewayForm.value.note === "undefined")
        ? this.gatewayForm.patchValue({ 'note': "" })
        : "";
      let successMessage = this._appConstant.msgCreated.replace("modulename", "Device");
      if (this.isEdit) {
        this.gatewayForm.registerControl("guid", new FormControl(''));
        this.gatewayForm.patchValue({ "guid": this.gatewayGuid });
        successMessage = this._appConstant.msgUpdated.replace("modulename", "Device");
      }
      this.gatewayService.addUpdategateway(this.gatewayForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/gateways']);
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

  getgatewayDetails(gatewayGuid) {
    this.spinner.show();
    this.gatewayService.getgatewayDetails(gatewayGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.gatewayObject = response.data;
        this.gatewayObject['templateGuid'] = (response.data.templateGuid) ? response.data.templateGuid.toUpperCase() : null;
        this.gatewayObject['greenHouseGuid'] = (response.data.greenHouseGuid) ? response.data.greenHouseGuid.toUpperCase() : null;
        // this.fileUrl = this.gatewayObject['image'];
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
}
