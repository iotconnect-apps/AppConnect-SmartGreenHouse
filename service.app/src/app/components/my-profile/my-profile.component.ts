import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, UserService, AuthService } from 'app/services';
import { AppConstant } from "../../app.constants";
import { CustomValidators } from 'app/helpers/custom.validators';


@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css']
})
export class MyProfileComponent implements OnInit {
  isAdmin = false;
  public contactNoError:boolean=false;
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
    public authService: AuthService,
    public _appConstant: AppConstant
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.createFormGroup();
  }

  ngOnInit() {
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.isAdmin = currentUser.userDetail.isAdmin;
    console.log(this.isAdmin);

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
      timeZoneGuid: ['', this.currentUser.userDetail.isAdmin ? '' : Validators.required],
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
    let contactNo = this.userForm.value.contactNo.replace("(", "")
    let contactno = contactNo.replace(")", "")
    let finalcontactno = contactno.replace("-", "")
    if(finalcontactno.match(/^0+$/)){
      this.contactNoError=true;
      return
    } else {
      this.contactNoError=false;
    }
    if (this.userForm.status === "VALID") {
      this.spinner.show();
      let contactNo = this.userForm.value.contactNo.replace("(", "");
      contactNo = contactNo.replace(")", "");
      if (!this.currentUser.userDetail.isAdmin) {
        this.userForm.get('id').setValue(this.currentUser.userDetail.id.toLowerCase());
        this.userForm.get('entityGuid').setValue(this.currentUser.userDetail.entityGuid.toLowerCase());
        this.userForm.get('email').setValue(this.userObject.email);
        this.userForm.get('roleGuid').setValue(this.currentUser.userDetail.roleId);
        this.userForm.get('contactNo').setValue(contactNo);
        this.userForm.get('firstName').setValue(this.userObject.firstName);
        this.userForm.get('lastName').setValue(this.userObject.lastName);
      }
      this.userService.addUser(this.currentUser.userDetail.isAdmin ? this.adminModel() : this.userForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.authService.changeUserNameData(this.adminModel().firstName + ' ' + this.adminModel().lastName);
          let curUser = JSON.parse(localStorage.getItem('currentUser'));
          curUser.userDetail.fullName = this.adminModel().firstName + ' ' + this.adminModel().lastName;
          localStorage.setItem('currentUser', JSON.stringify(curUser));
          this._appConstant.username = curUser.userDetail.fullName;
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

  /**
   * user Model
   * */
  userModel() {
    return {
      "id": this.userObject.guid,
      "firstName": this.userObject.firstName,
      "lastName": this.userObject.lastName,
      "email": this.userObject.email,
      "timeZoneGuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "imageName": "string",
      "contactNo": this.userObject.contactNo,
      "isActive": true,
      "isDeleted": true,
      "entityGuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "roleGuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  }

  /**
   * admin Model
   * */
  adminModel() {
    return {
      "id": this.userObject.guid,
      "firstName": this.userObject.firstName,
      "lastName": this.userObject.lastName,
      "email": this.userObject.email,
      "contactNo": this.userObject.contactNo,
      "password": this.userObject.password
    }

  }

}
