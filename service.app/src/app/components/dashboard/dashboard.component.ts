import * as moment from 'moment-timezone'
import { Component, OnInit, ViewChild } from '@angular/core'
import { NgxSpinnerService } from 'ngx-spinner'
import { DashboardService } from 'app/services/dashboard/dashboard.service';
import { Notification, NotificationService } from 'app/services';
import { GoogleChartComponent } from 'ng2-google-charts'

@Component({
	selector: 'app-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrls: ['./dashboard.component.css'],
})

export class DashboardComponent implements OnInit {
	lat = 23.033863;
	lng = 72.585022;
	@ViewChild('cchart', { static: false }) cchart: GoogleChartComponent;
	columnArray: any = [];
	headFormate: any = {
		columns: this.columnArray,
		type: 'NumberFormat'
	};
	bgColor = '#fff';
	chartHeight = 300;
	chartWidth = '100%';
	chart = {
		'waterConsumption': {
			chartType: 'ColumnChart',
			dataTable: [],
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				interpolateNulls: true,
				backgroundColor: this.bgColor,
				hAxis: {
					title: 'Date/Time',
					gridlines: {
						count: 5
					},
				},
				vAxis: {
					title: 'Values',
					gridlines: {
						count: 1
					},
				}
			},
			formatters: this.headFormate
		},
		'energyConsumption': {
			chartType: 'ColumnChart',
			dataTable: [],
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				interpolateNulls: true,
				backgroundColor: this.bgColor,
				hAxis: {
					title: 'Date/Time',
					gridlines: {
						count: 5
					},
				},
				vAxis: {
					title: 'Values',
					gridlines: {
						count: 1
					},
				}
			},
			formatters: this.headFormate
		},
		'soilNutritions': {
			chartType: 'ColumnChart',
			dataTable: [],
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				interpolateNulls: true,
				backgroundColor: this.bgColor,
				hAxis: {
					title: 'Date/Time',
					gridlines: {
						count: 5
					},
				},
				vAxis: {
					title: 'Values',
					gridlines: {
						count: 1
					},
				}
			},
			formatters: this.headFormate
		}
	};
	isShowLeftMenu = true;
	totalGreenhouse: any;
	totalCorp: any;
	totalConnectedDevices: any;
	totalDisconnectedDevices: any;
	totalAlerts: any;
	greenhouse = [];
	alerts: any = [];
	energyUsage: any;
	humidity: any;
	moisture: any;
	temperature: any;
	totalDevices: any;
	waterUsage: any;
	currentUser = JSON.parse(localStorage.getItem("currentUser"));
	selectedGreenhouseId = '';
	constructor(
		private spinner: NgxSpinnerService,
		public dashboardService: DashboardService,
		private _notificationService: NotificationService
	) { }

	ngOnInit() {
		this.getDashbourdCount()
		this.getGreenhouse();
		this.getWaterConsumptionChartData();
		this.getEnergyUsageChartData();
		this.getSoilnutritionChartData();
		this.getAlertList();
	}

	getAlertList() {
		let searchParameters = {
			pageNumber: 0,
			pageSize: 10,
			searchText: '',
			sortBy: 'eventDate desc',
			deviceGuid: '',
			entityGuid: '',
		  };
		this.spinner.show();
		this.dashboardService.getAlertsList(searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true && response.data.items) {
				this.alerts = response.data.items;
			}
			else {
				this.alerts = [];
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.alerts = [];
			this._notificationService.add(new Notification('error', error));
		});
	}


	/**
	 * Get data for water consumpation chart
	 * */
	getWaterConsumptionChartData() {
		let obj = { companyGuid: this.currentUser.userDetail.companyId };
		let data = [];
		this.dashboardService.getWaterUsageChartData(obj).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['Months', 'Water Consumption']);
				}
				response.data.forEach(element => {
					data.push([element.month, parseInt(element.value)]);
				});
				this.createChart('waterConsumption', data, 'Months', 'gal');
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});


	}

	getEnergyUsageChartData() {
		let obj = { companyGuid: this.currentUser.userDetail.companyId };
		let data = []
		this.dashboardService.getEnergyUsageChartData(obj).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['Months', 'Energy']);
				}
				response.data.forEach(element => {
					data.push([element.month, parseInt(element.value)]);
				});
				this.createChart('energyConsumption', data, 'Months', 'KWH');
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});


	}

	getSoilnutritionChartData() {
		let obj = { companyGuid: this.currentUser.userDetail.companyId };
		let data = []
		this.dashboardService.getSoilnutritionChartData(obj).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['pH Level', 'N', 'P', 'K']);
				}
				response.data.forEach(element => {
					data.push([element.phLevel, parseInt(element['n']), parseInt(element['p']), parseInt(element['k'])]);
				});
				this.createChart('soilNutritions', data, 'pH Level', '% Availability');
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});


	}
	createChart(key, data, hAxisTitle, vAxisTitle) {
		this.chart[key] = {
			chartType: 'ColumnChart',
			dataTable: data,
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				interpolateNulls: true,
				backgroundColor: this.bgColor,
				hAxis: {
					title: hAxisTitle,
					gridlines: {
						count: 5
					},
				},
				vAxis: {
					title: vAxisTitle,
					gridlines: {
						count: 1
					},
				}
			},
			formatters: this.headFormate
		};
	}

	getDashbourdCount() {
		this.spinner.show();
		this.dashboardService.getDashboardoverview().subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.totalGreenhouse = response.data.greenHouseCount
				this.totalCorp = response.data.cropCount
				this.totalConnectedDevices = response.data.connectedDeviceCount
				this.totalDisconnectedDevices = response.data.disconnectedDeviceCount
				this.totalAlerts = (response.data.alertsCount) ? response.data.alertsCount : 0
			}
			else {
				this._notificationService.add(new Notification('error', response.message));

			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	getGreenhouse() {
		this.spinner.show();
		this.dashboardService.getActiveGreenHouse().subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.greenhouse = response.data;
				if (this.greenhouse.length > 0) {
					this.selectedGreenhouseId = this.greenhouse[0]['guid'];
					this.getgreenHouse(this.greenhouse[0]['guid']);
				}
			}
			else {
				this._notificationService.add(new Notification('error', response.message));

			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	getgreenHouse(greenhouseguid) {
		this.spinner.show();
		this.dashboardService.getGreenHouseDetail(greenhouseguid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.energyUsage = response.data.energyUsage
				this.humidity = response.data.humidity
				this.moisture = response.data.moisture
				this.temperature = response.data.temperature
				this.totalDevices = response.data.totalDevices
				this.waterUsage = response.data.waterUsage
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
