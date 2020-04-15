import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { MatDialog } from "@angular/material";
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { GreenHouseService, NotificationService, Notification } from "../../../services";


@Component({
	selector: "app-green-house-list",
	templateUrl: "./green-house-list.component.html",
	styleUrls: ["./green-house-list.component.css"]
})
export class GreenHouseListComponent implements OnInit {

	changeStatusDeviceName: any;
	changeStatusDeviceStatus: any;
	changeDeviceStatus: any;
	greenHouseList = [];
	moduleName = "Green Houses";
	order = true;
	isSearch = false;
	totalRecords = 5;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	reverse = false;
	orderBy = "name";
	searchParameters = {
		pageNumber: -1,
		pageSize: -1,
		searchText: "",
		sortBy: "name asc"
	};
	deleteAlertDataModel: DeleteAlertDataModel;
	displayedColumns: string[] = ["name", "address", "city", "zipcode", "description", "isActive", "action"];
	mediaUrl: any;

	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		public dialog: MatDialog,
		public greenHouseService: GreenHouseService,
		public _appConstant: AppConstant,
		private _notificationService: NotificationService
	) {
		this.mediaUrl = this._notificationService.apiBaseUrl
	}

	ngOnInit() {

		this.getgreenHouseList();
	}

	clickAdd() {
		this.router.navigate(["/green-house/add"]);
	}

	/**
	 * Set sort order 
	 * @param sort
	 */
	setOrder(sort: any) {
		console.log(sort);
		if (!sort.active || sort.direction === '') {
			return;
		}
		this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
		this.getgreenHouseList();
	}

	deleteModel(userModel: any) {
		this.deleteAlertDataModel = {
			title: "Delete GreenHouse",
			message: this._appConstant.msgConfirm.replace('modulename', "GreenHouse"),
			okButtonName: "Yes",
			cancelButtonName: "No",
		};
		const dialogRef = this.dialog.open(DeleteDialogComponent, {
			width: '400px',
			height: 'auto',
			data: this.deleteAlertDataModel,
			disableClose: false
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this.deletegreenHouse(userModel.guid);
			}
		});
	}

	onPageSizeChangeCallback(pageSize) {
		this.searchParameters.pageSize = pageSize;
		this.searchParameters.pageNumber = 1;
		this.isSearch = true;
		this.getgreenHouseList();
	}

	activeInactivegreenhouse(id: string, isActive: boolean, name: string) {
		var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
		var mapObj = {
			statusname: status,
			fieldname: name,
			modulename: "GreenHouse"
		};
		this.deleteAlertDataModel = {
			title: "Status",
			message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname/gi, function (matched) {
				return mapObj[matched];
			}),
			okButtonName: "Yes",
			cancelButtonName: "No",
		};
		const dialogRef = this.dialog.open(DeleteDialogComponent, {
			width: '400px',
			height: 'auto',
			data: this.deleteAlertDataModel,
			disableClose: false
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this.changeGreenhouseStatus(id, isActive);

			}
		});

	}

	/**
	 * 
	 * @param pagechangeresponse
	 */
	ChangePaginationAsPageChange(pagechangeresponse) {
		this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
		this.searchParameters.pageSize = pagechangeresponse.pageSize;
		this.isSearch = true;
		this.getgreenHouseList();
	}

	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNumber = 0;
		this.getgreenHouseList();
		this.isSearch = true;
	}

	getgreenHouseList() {
		this.spinner.show();
		this.greenHouseService.getgreenHouse(this.searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.totalRecords = response.data.count;
				// this.isSearch = false;
				this.greenHouseList = response.data.items;
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
				this.greenHouseList = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	deletegreenHouse(guid) {
		this.spinner.show();
		this.greenHouseService.deletegreenHouse(guid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Greenhouse")));
				this.getgreenHouseList();

			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}

		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	changeGreenhouseStatus(id, isActive) {

		this.spinner.show();
		this.greenHouseService.changeStatus(id, isActive).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Greenhouse")));
				this.getgreenHouseList();

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
