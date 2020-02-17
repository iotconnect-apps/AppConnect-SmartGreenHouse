import { Component, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, LookupService, Notification, NotificationService, AuthService } from 'app/services'
import { RxFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { RequestSubscriberFormModel } from './subscriber.model';
import { PurchasePlanComponent } from '../purchase-plan/purchase-plan.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, AfterViewInit {
  isFormSubmittedSuccessfully: any;
  @ViewChild(PurchasePlanComponent, { static: false }) child: PurchasePlanComponent;

  modulename = "Register";
  SubscriberFormGroup: RxFormGroup;
  SubscriberData: RequestSubscriberFormModel = new RequestSubscriberFormModel();
  checkSubmitStatus = false;
  subscribeObject = {};
  countryList = [];
  stateList = [];
  timezoneList = [];
  isSubmitted = false;
  isShowPayment = false;
  loginStatus = false;
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    private _notificationService: NotificationService,
    public userService: UserService,
    public lookupService: LookupService,
    private formBuilder: RxFormBuilder,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loginStatus = this.authService.isCheckLogin();
    if (this.loginStatus === true) {
      this.router.navigate(['dashboard']);
    }
    this.getTimezoneList();
    this.getcountryList();
    this.createFormGroup();
    //this.userService.logout();
  }

  ngAfterViewInit() {
    // console.log(this.child); // 
  }


  /**
   * Create reactive form group
   */
  createFormGroup() {
    this.SubscriberFormGroup = <RxFormGroup>this.formBuilder.formGroup(this.SubscriberData);
    this.SubscriberFormGroup.patchValue({
      //"address": "aghmedabad",
      //"cityName": "Ahmedabad",
      //"companyName": "aBC",
      //"cpId": "Mandip",
      //"email": "mandip@mailinator.com",
      //"firstName": "Mandip",
      //"lastName": "Vora",
      //"password": "Softweb#123",
      //"ccode": "123",
      //"phoneNumber": "7878787878",
      //"postalCode": "7878787",
      //"stateId": "3092b89e-e71c-4b43-a679-413cd3844c78",
      //"timezoneId": "8b7a9755-3470-49ed-ba2d-24b5b1bb9b7e"
    })
  }

  /**
   * Get timezone list
   */
  getTimezoneList() {
    this.spinner.show();
    this.lookupService.getNoAuthTimezoneList().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.timezoneList = response.data.data;
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
   * Get country list
   */
  getcountryList() {
    this.spinner.show();
    this.lookupService.getNoAuthcountryList().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.countryList = response.data.data;
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
   * Get state list based on country id
   * 
   * @param event 
   */
  changeCountry(event) {
    let id = event.value;
    if (id) {
      this.spinner.show();
      this.lookupService.getNoAuthcitylist(id).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.stateList = response.data.data;
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

  /**
   * Do next step payment once subscription form is completed
   */
  doPayment() {
    this.isSubmitted = true;
    if (!this.SubscriberFormGroup.valid) {
      return;
    }
    if (this.isShowPayment) {
      this.child.onSubmitPayment()
        .catch(err => console.log(err))
        .then((result: any) => (typeof result !== "undefined") ? this.registerUser(result) : false);
    }
    this.isShowPayment = true;
  }

  goBack() {
    this.isShowPayment = !this.isShowPayment
  }
  registerUser(data) {
    let postData = {
      "subscriptionToken": data.subscriptionToken,
      "solutionCode": data.solutionCode,
      "solutionPlanCode": data.solutionPlanCode,
      "isAutoRenew": data.isAutoRenewal,
      "stripeToken": data.stripeToken,
      "stripeCardId": data.stripeCardId,
      "subscriberId": data.subscriberId,
      "user": {
        "firstName": this.SubscriberFormGroup.value['firstName'],
        "lastName": this.SubscriberFormGroup.value['lastName'],
        "email": this.SubscriberFormGroup.value['email'],
        "password": this.SubscriberFormGroup.value['password'],
        "phone": this.SubscriberFormGroup.value['ccode'] + '-' + this.SubscriberFormGroup.value['phoneNumber'],
        "companyName": this.SubscriberFormGroup.value['companyName'],
        "address": this.SubscriberFormGroup.value['address'],
        "timezoneId": this.SubscriberFormGroup.value['timezoneId'],
        "countryId": this.SubscriberFormGroup.value['countryName'],
        "stateId": this.SubscriberFormGroup.value['stateName'],
        "cityName": this.SubscriberFormGroup.value['cityName'],
        "postalCode": this.SubscriberFormGroup.value['postalCode'],
        "cpId": ""
      }
    };
    this.spinner.show();
    this.lookupService.postNoAuthRegister(postData).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', 'Registered successfully'));
        this.router.navigate(['/login']);
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
