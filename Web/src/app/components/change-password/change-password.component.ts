import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, UserService } from 'app/services';
import { CustomValidators } from 'app/helpers/custom.validators';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  userForm: FormGroup;
  moduleName = "Change Password";
  buttonname = "Update";
  checkSubmitStatus = false;
  currentUser: any = {};
  userObject: any = {};

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public userService: UserService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.createFormGroup();
  }

  ngOnInit() {
    this.getCurrentUserInfo();
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
   * Create reactive form group
   */
  createFormGroup() {
    this.userForm = this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [
          Validators.required, 
          Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])[A-Za-z\d$@$!@#$%^&*].{5,}')


        ]
      ],
      confirmPassword: ['', Validators.required]
    }, {
      validator: CustomValidators.matchPwds('newPassword', 'confirmPassword')
    });
  }


  log(obj) {
    console.log(obj);
  }


  /**
   * change user password
   */

  changePassword() {
    this.checkSubmitStatus = true;
    if (this.userForm.status === "VALID") {
      this.spinner.show();
      if(!this.userObject){
         this._notificationService.add(new Notification('error', "User not found"));
         return false;
      }
      this.userForm.registerControl("email", new FormControl(''));
      this.userForm.patchValue({"email" : this.userObject.email});
      this.userService.changePassword(this.userForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this._notificationService.add(new Notification('success', "Password has been changed successfully."));
          this.router.navigate(['/dashboard']);
        } else {
          this.userForm.reset();
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this.userForm.reset();
        this._notificationService.add(new Notification('error', error));
      })
    }
  }

}
