import { Component, OnInit,ViewChild,ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { DeviceService, NotificationService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant } from "../../../../app.constants";
import { Guid } from "guid-typescript";
import {MatPaginator} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import { saveAs } from 'file-saver';
export interface DeviceTypeList {
	id: number;
	type: string;
}
export interface deviceobj {
	deviceid: any;
}
@Component({
	selector: 'app-bulkupload-add',
	templateUrl: './bulk-upload-add.component.html',
	styleUrls: ['./bulk-upload-add.component.css']
})


export class BulkuploadAddComponent implements OnInit {
	@ViewChild('profile_picture_Ref', { static: false }) profile_picture_Ref: ElementRef;
	tblshow = false;
	formshow = false;
	IsForUpdate = false;
	kitDevices = [];
	parentdevicearry = [];
	deviceobj ={};
	currentUser: any;
	fileUrl: any;
	fileName = '';
	fileToUpload: any = null;
	status;
	moduleName = "Bulk Upload";
	deviceObject = {};
	hardwareGuid = '';
	isEdit = false;
	bulkForm: FormGroup;
	checkSubmitStatus = false;
	buildingList = [];
	selectedBuilding = '';
	floorList = [];
	spaceList = [];
	templateList = [];
	dataSource:any = [];
	//displayedColumns: string[] = ['Device ID','Device Name','Tag','action'];
	displayedColumns: string[] = ['kitCode','uniqueId','name','tag','action','message'];
	@ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
	
	index: any = -1;
	parentDeviceGuids: string;
	jsonadata: any;
	dataobj: any;
	responseobj: any;
	constructor(
		private router: Router,
		private _notificationService: NotificationService,
		private activatedRoute: ActivatedRoute,
		private spinner: NgxSpinnerService,
		private deviceService: DeviceService,
		public _appConstant: AppConstant
	) {
		this.createFormGroup();
	}

	ngOnInit() {
	     this.gettemplateLookup();
	}

	download(){
		this.deviceService.getHardwarkitDownload().subscribe(response => {
		var myJSON = JSON.stringify(response);
		saveAs(new Blob([myJSON], { type: "json" }), 'data.json');
		});
		//var obj = { name: "John", age: 30, city: "New York" };
		//var myJSON = JSON.stringify(obj);
		//saveAs(new Blob([myJSON], { type: "json" }), 'data.json');
	}

	createFormGroup() {
		this.bulkForm = new FormGroup({
			profile_picture: new FormControl('',[Validators.required]),
			kitTypeGuid:new FormControl('',[Validators.required]),
		});
	}

	uploadbulk() {
		this.checkSubmitStatus = true;
		if (this.bulkForm.status === "VALID") {
			this.spinner.show();
			this.jsonadata = {"kitTypeGuid":this.bulkForm.value.kitTypeGuid,"hardwareKits":this.dataobj}
			this.deviceService.uploadFile(this.jsonadata).subscribe(response => {
				this.responseobj = response.data;
				this.dataSource=new MatTableDataSource(response.data);
				this.dataSource.paginator = this.paginator;
				this.tblshow = true
				this.formshow = true
				//console.log("dassss",response)
				this.spinner.hide();
				//this.router.navigate(['/profile']);
				this._notificationService.add(new Notification('success', " File upload successfully."));
			});
		}
	}

	removeFile(type) {
		if (type === 'image') {
			this.fileUrl = '';
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
			const f = event.target.files[0];
			const reader = new FileReader();
	
		reader.onload = ((theFile) => {
		  return (e) => {
			try {
			  const json = JSON.parse(e.target.result);
			  const resSTR = JSON.stringify(json);
			  this.dataobj = JSON.parse(resSTR);
			  //this.jsonadata = JSON.parse(resSTR);
			} catch (ex) {
			}
		  };
		})(f);
		reader.readAsText(f);
		}
	}

	gettemplateLookup() {
		this.deviceService.getkittypes().subscribe(response => {
			this.templateList = response['data'];
		});
	}
	Upload(){
		this.deviceService.uploadData(this.jsonadata).subscribe(response => {
			this.tblshow = false;
			this.formshow = false;
			//console.log("dassss",response)
			this.spinner.hide();
			this.gettemplateLookup();
			this.checkSubmitStatus = false;
			this.bulkForm.reset();
			//this.router.navigate(['admin/bulkupload']);
			this._notificationService.add(new Notification('success', " File upload successfully."));
		});
	}
	Cancel(){
		this.checkSubmitStatus = false;
		this.tblshow = false;
		this.formshow = false;	
		this.bulkForm.reset();
	}
}
