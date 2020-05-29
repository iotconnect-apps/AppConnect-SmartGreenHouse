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
	bgColor = ['#5496d0'];
	chartHeight = 350;
	chartWidth = '100%';
	chart = {
		'waterConsumption': {
			chartType: 'ColumnChart',
			dataTable: [],
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				legend: { position: 'none' },
				interpolateNulls: true,
				backgroundColor: this.bgColor,
				colors:this.bgColor,
				hAxis: {
					title: 'Date/Time',
					gridlines: {
						count: 12
					},
					maxAlternation: 1, // use a maximum of 1 line of labels
					showTextEvery: 1, // show every label if possible
					minTextSpacing: 8 // minimum spacing between adjacent labels, in pixels
					
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
				legend: { position: 'none' },
				backgroundColor: this.bgColor,
				colors:this.bgColor,
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
				colors:'red',
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
	fieldcolor:any;
	isShowLeftMenu = true;
	totalGreenhouse: any;
	totalCorp: any;
	totalConnectedDevices: any;
	totalDisconnectedDevices: any;
	totalAlerts: any;
	greenhouse = [];
	alerts: any = [];
	stats = {
		energyUsage: 0,
		humidity: 0,
		moisture: 0,
		temperature: 0,
		totalDevices: 0,
		waterUsage: 0
	}
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
			//this.spinner.hide();
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
			//this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['Months', 'Water Consumption']);
				}
				response.data.forEach(element => {
					data.push([element.month, parseFloat(element.value)]);
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
			//this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['Months', '']);
				}
				response.data.forEach(element => {
					data.push([element.month, parseFloat(element.value)]);
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
			//this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['pH Level', 'N', 'P', 'K']);
				}
				response.data.forEach(element => {
					data.push([element.day, parseFloat(element['n']), parseFloat(element['p']), parseFloat(element['k'])]);
				});
				this.createChart('soilNutritions', data, 'Days', '% pH Level');
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
		let legend = { position: 'none' };
		var hAxis={};
		if (key === 'soilNutritions') {
			legend = { position: 'right' };
			hAxis={
				title: hAxisTitle,
				gridlines: {
					count: 5
				},
				// slantedText:true,
				// slantedTextAngle:45,
			}
		} else {
			hAxis={
				title: hAxisTitle,
				gridlines: {
					count: 5
				},
				slantedText:true,
				slantedTextAngle:45,
			}
		}
		if (key === 'energyConsumption') {
		this.chart[key] = {
			chartType: 'ColumnChart',
			dataTable: data,
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				interpolateNulls: true,
				legend: legend,
				backgroundColor: this.bgColor,
				colors: ['#ed734c'],
				hAxis: hAxis,
				vAxis: {
					title: vAxisTitle,
				}
			},
			formatters: this.headFormate
		};
		}else{
		this.chart[key] = {
			chartType: 'ColumnChart',
			dataTable: data,
			options: {
				height: this.chartHeight,
				width: this.chartWidth,
				interpolateNulls: true,
				legend: legend,
				backgroundColor: this.bgColor,
				hAxis: hAxis,
				vAxis: {
					title: vAxisTitle,
				}
			},
			formatters: this.headFormate
		};
		}
		
	}

	getDashbourdCount() {
		this.spinner.show();
		this.dashboardService.getDashboardoverview().subscribe(response => {
			//this.spinner.hide();
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
			//this.spinner.hide();
			if (response.isSuccess === true) {
				this.greenhouse = response.data;
				if (this.greenhouse.length > 0) {
					this.selectedGreenhouseId = this.greenhouse[0]['guid'];
					this.getgreenHouse(this.greenhouse[0]['guid']);
				} else {
					this.spinner.hide();
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

	getLocalDate(lDate) {
		var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
		// Get the local version of that date
		var localDate = moment(utcDate).local();
		let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
		return res;

	}


	getgreenHouse(greenhouseguid) {
		this.spinner.show();
		this.dashboardService.getGreenHouseDetail(greenhouseguid).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.stats.energyUsage = (response.data.totalEnergyCount) ? response.data.totalEnergyCount : 0;
				this.stats.humidity = (response.data.avgHumidity) ? response.data.avgHumidity : 0;
				this.stats.moisture = (response.data.avgMoisture) ? response.data.avgMoisture : 0;
				this.stats.temperature = (response.data.avgTemp) ? response.data.avgTemp : 0;
				this.stats.totalDevices = (response.data.totalDevice) ? response.data.totalDevice : 0;
				this.stats.waterUsage = (response.data.totalWaterUsage) ? response.data.totalWaterUsage : 0;
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
