import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, AuthService  } from '../../../services/index'
import { Notification, NotificationService } from 'app/services';


@Component({
	selector: 'app-admin-login',
	templateUrl: './admin-login.component.html',
	styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent implements OnInit {
	loginform: FormGroup;
	checkSubmitStatus = false;
	loginObject = {};
	loginStatus = false;
	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		private _notificationService: NotificationService,
		public UserService : UserService,
		private authService: AuthService
	) { }

	ngOnInit() {
		this.loginStatus = this.authService.isCheckLogin();
		if (this.loginStatus === true) {
		  this.router.navigate(['admin/dashboard']);
		}
		this.createFormGroup();
		// logout the person when he opens the app for the first time
		//this.UserService.logout();
	}

	createFormGroup() {
		this.loginform = new FormGroup({
			username: new FormControl('',[Validators.required,Validators.email]),
			password: new FormControl('',[Validators.required]),
		});

		this.loginObject = {
			username: '',
			password: '',
		};
	}

	login() {
		this.checkSubmitStatus = true;
		if (this.loginform.status === "VALID" && this.checkSubmitStatus) {
			this.spinner.show();
			this.UserService.loginadmin(this.loginObject).subscribe(response => {
				this.spinner.hide();
				if (response.isSuccess === true && response.data.access_token) {
					this._notificationService.add(new Notification('success', 'Logged in successfully'));
					this.router.navigate(['admin/dashboard']);
				}
				else {
					this._notificationService.add(new Notification('error', response.message));
					this.router.navigate(['/admin']);
				}
			}, error => {
				this.spinner.hide();
				this._notificationService.add(new Notification('error', error ));
			});
		}
	}

	forgotPassword($event) {
		$("#divLoginSection").hide();
		$("#divForgotPwdSection").show();
	}
	
	forgotPasswordCancel($event) {
		$("#divForgotPwdSection").hide();
		$("#divLoginSection").show();

	}
}