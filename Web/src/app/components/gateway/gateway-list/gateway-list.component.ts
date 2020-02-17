import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatTableDataSource, MatSort, MatPaginator } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { NotificationService, GatewayService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";

@Component({ selector: 'app-gateway-list', templateUrl: './gateway-list.component.html', styleUrls: ['./gateway-list.component.scss'] })

export class GatewayListComponent implements OnInit {
	changeStatusgatewayName:any;
	changeStatusgatewayStatus:any;
	changegatewayStatus:any;
	modulename = "Hardware Kit";
	order = true;
	isSearch = false;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	reverse = false;
	orderBy = 'uniqueId';
	totalRecords = 0;
	searchParameters = {
		pageNumber: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'uniqueId asc'
	};
	displayedColumns: string[] = ['uniqueId', 'name', 'greenHouseName', 'count', 'isActive'];
	dataSource = [];
	deleteAlertDataModel: DeleteAlertDataModel;

	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		public dialog: MatDialog,
		private gatewayService: GatewayService,
		private _notificationService: NotificationService,
		public _appConstant: AppConstant
	) { }

	ngOnInit() {
		this.getgatewayList();
	}

	clickAdd() {
		this.router.navigate(['/gateways/add']);
	}

	setOrder(sort: any) {
		if (!sort.active || sort.direction === '') {
			return;
	   }
	  	this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
		this.getgatewayList();
	}

	deleteModel(gatewayModel: any) {
		this.deleteAlertDataModel = {
			title: "Delete Device",
			message: this._appConstant.msgConfirm.replace('modulename', "Hardware Kit"),
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
				this.deletegateway(gatewayModel.guid);
			}
		});
	}

	
	ChangePaginationAsPageChange(pagechangeresponse) {
		this.searchParameters.pageSize = pagechangeresponse.pageSize;
		this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
		this.isSearch = true;
		this.getgatewayList();
	}

	searchTextCallback(filterText) {
		this.isSearch = true;
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNumber = 0;
		this.getgatewayList();
	}

	// openChangeStatusDialog(guid, name, status) {
	// 	// this.changeStatusgatewayGuid = guid;
	// 	// this.changeStatusgatewayName = name;
	// 	// this.changeStatusgatewayStatus = status;
	// }

	// openDeleteDialog(guid, name) {
	// 	// this.changeStatusgatewayGuid = guid;
	// 	// this.changeStatusgatewayName = name;
	// }

	getgatewayList() {
		this.spinner.show();
		this.gatewayService.getgateways(this.searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.totalRecords = response.data.count;
				// this.isSearch = false;
      //  debugger;
        this.dataSource = response.data.items;
        console.log(this.dataSource);
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
				this.dataSource = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error ));
		});
	}

  activeInactiveGateway(gatewayId: string, isActive: boolean, name: string) {
		var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
		var mapObj = {
			statusname: status,
			fieldname: name,
			modulename: "device"
		};
		this.deleteAlertDataModel = {
			title: "Status",
			message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname|modulename/gi, function (matched) {
				return mapObj[matched];
			}),
			okButtonName: "Confirm",
			cancelButtonName: "Cancel",
		};
		const dialogRef = this.dialog.open(DeleteDialogComponent, {
			width: '400px',
			height: 'auto',
			data: this.deleteAlertDataModel,
			disableClose: false
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this.changeGatewayStatus(gatewayId, isActive);

			}
		});

	}
	changeGatewayStatus(gatewayId, isActive) {
   // debugger;
		this.spinner.show();
    this.gatewayService.changeStatus(gatewayId, false).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Hardware kit")));
				this.getgatewayList();
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}

		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}
	deletegateway(guid) {
		this.spinner.show();
		this.gatewayService.deletegateway(guid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Hardware kit")));
				this.getgatewayList();
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
