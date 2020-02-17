import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormArray } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { GatewayService, NotificationService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant } from "../../../app.constants";

@Component({
  selector: 'app-gateway-add',
  templateUrl: './gateway-add.component.html',
  styleUrls: ['./gateway-add.component.css']
})

export class GatewayAddComponent implements OnInit {

  count: number = 0;
  currentUser: any;
  moduleName = "ADD HARDWARE KIT";
  isEdit = false;
  gatewayForm: FormGroup;
  checkSubmitStatus = false;
  isNext = false;
  greenhouseList = [];

  angForm = new FormGroup({
    names: new FormArray([
    ])
  });

  get names(): FormArray {
    return this.angForm.get('names') as FormArray;
  }

  addNameField() {
    this.names.push(new FormControl('', Validators.required));
  }

  arrayItems: {
    uniqueId: any;
  }[];

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

    });
  }

  ngOnInit() {
    this.getGreenHouseLookup();
  }

  /**
   * form
   * */
  createFormGroup() {
    this.gatewayForm = new FormGroup({
      kitCode: new FormControl('', [Validators.required]),
      greenHouseGuid: new FormControl('', [Validators.required]),
      kitDevices: new FormArray([
      ]),
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

  /**
	 * 
	 * Get getway count
	*/
  getGatewayCount(kitCode) {
    this.spinner.show();
    if (this.gatewayForm.status === "VALID") {
      this.gatewayService.getGatewayCount(kitCode).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.count = response['data'];
          if (this.count > 0) {
            for (let i = 0; i < this.count; i++) {
              this.addNameField();
            }
          }
        }
        else {
          this.isNext = false;
          this._notificationService.add(new Notification('error', 'Kit not found'));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      });
    }
    else {
      this.isNext = false;
    }
    this.spinner.hide();
  }

  /**
   * Refresh the page
   * */
  refresh() {
    for (let i = this.count; i >= 0; i--) {
      this.names.removeAt(i);
    }
    this.isNext = false;
    this.count = 0;
    this.gatewayForm.reset();
  }

  /**
   * Post data
   * */
  addgateway() {
    this.checkSubmitStatus = true;
    for (let i = 0; i < this.names.length; i++) {
      this.gatewayForm.value.kitDevices.push(this.names.at(i).value);
    }
    if (this.gatewayForm.status === "VALID") {
      this.spinner.show();

      let successMessage = this._appConstant.msgAdded.replace("modulename", "Hardware kit");
      this.gatewayService.provisionKit(this.gatewayForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/gateways']);
          this._notificationService.add(new Notification('success', successMessage));
        } else {
          this.refresh();
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.refresh();
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      });
    }
  }

}
