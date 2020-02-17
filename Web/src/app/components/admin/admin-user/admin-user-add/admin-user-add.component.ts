import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, UserService, GatewayService } from 'app/services';
import {CustomValidators} from 'app/helpers/custom.validators';

@Component({
	selector: 'app-adminuser-add',
	templateUrl: './admin-user-add.component.html',
	styleUrls: ['./admin-user-add.component.css']
})
export class AdminUserAddComponent implements OnInit {
	public mask = {
		guide: true,
		showMask: false,
		keepCharPositions: true,
		mask: ['(', /[0-9]/, /\d/, ')', '-', /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/]
	};
	fileUrl: any;
	fileName = '';
	fileToUpload: any = null;
	status;
	moduleName = "Add User";
	userObject = {};
	userGuid = '';
	isEdit = false;
	userForm: FormGroup;
	checkSubmitStatus = false;
	buildingList = [];
	selectedBuilding = '';
	floorList = [];
	spaceList = [];
	roleList = [];
	cityList = [];
	buttonname = 'SUBMIT'
	arrystatus = [{ "name": "Active", "value": true }, { "name": "Inactive", "value": false }]
	timezoneList = [];
	enitityList = [];
	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private _notificationService: NotificationService,
		private activatedRoute: ActivatedRoute,
		private spinner: NgxSpinnerService,
		public userService: UserService,
		public gatewayService: GatewayService,
	) {
		this.activatedRoute.params.subscribe(params => {
			if (params.userGuid != null) {
				this.getUserDetails(params.userGuid);
				this.userGuid = params.userGuid;
				this.moduleName = "Edit User";
				this.isEdit = true;
				this.buttonname = 'UPDATE'
			} else {
				this.userObject = { firstName: '', lastName: '', email: '', contactNo: '', password: '' }
			}
		});
		this.createFormGroup();
	}

	ngOnInit() {

	}

	createFormGroup() {

			this.userForm = this.formBuilder.group({
			    id:[''],
				firstName: ['', Validators.required],
				lastName: ['', Validators.required],
				email: ['', [Validators.required, Validators.email]],
				contactNo: ['', Validators.required],
				password: ['', Validators.required],
				companyGuid:['']
			},{
				validators : CustomValidators.checkPhoneValue('contactNo')
			});
	}
	
	addUser() {
		let contactNo = this.userForm.value.contactNo.replace("(", "")
		let contactno = contactNo.replace(")", "")
		this.checkSubmitStatus = true;
		this.userForm.get('id').setValue('00000000-0000-0000-0000-000000000000');
		if (this.userForm.status === "VALID") {
			if (this.isEdit) {
				this.userForm.registerControl("id", new FormControl(''));
				this.userForm.patchValue({"id" : this.userGuid});
			}
			this.spinner.show();
			 let currentUser = JSON.parse(localStorage.getItem('currentUser'));
			//this.userForm.get('entityGuid').setValue(currentUser.userDetail.entityGuid);
			this.userForm.get('companyGuid').setValue(currentUser.userDetail.companyId);
			this.userForm.get('contactNo').setValue(contactno);
			this.userService.addadminUser(this.userForm.value).subscribe(response => {
				if (response.isSuccess === true) {
					this.spinner.hide();
					if (response.data.updatedBy != null) {
						this._notificationService.add(new Notification('success', "User has been updated successfully."));
					} else {
						this._notificationService.add(new Notification('success', "User has been added successfully."));
					}
					this.router.navigate(['/admin/viewusers']);
				} else {
					this.spinner.hide();
					this._notificationService.add(new Notification('error', response.message));
				}
			})
		}
	}
	getUserDetails(userGuid) {
		this.spinner.show();
		this.userService.getadminUserDetails(userGuid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.userObject = response.data;
				//this.fileUrl = this.deviceObject['image'];
			}
		});
	}
	getdata(val) {
		return val = val.toLowerCase();
	}
}
