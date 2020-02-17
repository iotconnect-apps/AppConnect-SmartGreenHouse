import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, UserService } from 'app/services';
import { AppConstant } from "../../app.constants";
import {CustomValidators} from 'app/helpers/custom.validators';


@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css']
})
export class MyProfileComponent implements OnInit {

  public mask = {
    guide: true,
    showMask: false,
    keepCharPositions: true,
    mask: ['(', /[0-9]/, /\d/, ')', '-', /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/]
  };
  userForm: FormGroup;
  userObject: any = {};
  moduleName = "My Profile";
  buttonname = 'Save'
  timezoneList: any = [];
  checkSubmitStatus = false;
  currentUser: any = {};


  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public userService: UserService,
    public _appConstant: AppConstant
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.createFormGroup();
  }

  ngOnInit() {
    this.getCurrentUserInfo();
    this.getTimezoneList();
  }

  /**
    * Create reactive form group
    */
  createFormGroup() {
    this.userForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      contactNo: ['', Validators.required],
      timeZoneGuid: ['', Validators.required],
      id: [null],
      entityGuid: [null],
      email: [null],
      roleGuid: [null]
    }, {
      validator: CustomValidators.checkPhoneValue('contactNo')
    });
  }

  /**
   * Get current user info
   */
  getCurrentUserInfo() {
    this.spinner.show();
    let userGuid = this.currentUser.userDetail.id;
    this.userService.getUserDetails(userGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.userObject = response.data;
      } else {
        this._notificationService.add(new Notification('error', response.message));
        this.userObject = {};
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
  /**
   * Get timezone list lookup
   */
  getTimezoneList() {
    this.spinner.show();
    this.userService.getTimezoneList().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.timezoneList = response.data;
      } else {
        this._notificationService.add(new Notification('error', response.message));
        this.timezoneList = {};
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });


  }

  /**
   * Update user info
   */

  updateUser() {
    this.checkSubmitStatus = true;
    if (this.userForm.status === "VALID") {
      this.spinner.show();
      let contactNo = this.userForm.value.contactNo.replace("(", "");
      contactNo = contactNo.replace(")", "");
      this.userForm.get('id').setValue(this.currentUser.userDetail.id.toLowerCase());
      this.userForm.get('entityGuid').setValue(this.currentUser.userDetail.entityGuid.toLowerCase());
      this.userForm.get('email').setValue(this.userObject.email);
      this.userForm.get('roleGuid').setValue(this.currentUser.userDetail.roleId);
      this.userForm.get('contactNo').setValue(contactNo);
      this.userService.addUser(this.userForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          let successMessage = this._appConstant.msgUpdated.replace("modulename", "Profile");
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

}
