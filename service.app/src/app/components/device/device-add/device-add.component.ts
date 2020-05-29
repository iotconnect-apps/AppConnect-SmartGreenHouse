import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { DeviceService, NotificationService, GatewayService, LookupService, RuleService, DashboardService } from 'app/services';
import { Notification } from 'app/services/notification/notification.service';
import { AppConstant } from "../../../app.constants";
import { Guid } from "guid-typescript";
import { Observable, forkJoin } from 'rxjs';
import { StompRService } from '@stomp/ng2-stompjs'
import { Message } from '@stomp/stompjs'
import { Subscription } from 'rxjs'
import { environment } from 'environments/environment'
import * as moment from 'moment-timezone'
import 'chartjs-plugin-streaming';
import { DatePipe } from '@angular/common'
import * as _ from 'lodash'

export interface DeviceTypeList {
	id: number;
	type: string;
}
export interface StatusList {
	id: boolean;
	status: string;
}
@Component({
	selector: 'app-device-add',
	templateUrl: './device-add.component.html',
	styleUrls: ['./device-add.component.css'],
	providers: [StompRService]
})


export class DeviceAddComponent implements OnInit {
	searchParameters = {
		pageNo: 0,
		pageSize: 80,
		searchText: '',
		sortBy: 'uniqueId asc'
	};
	chartColors: any = {
		red: 'rgb(255, 99, 132)',
		orange: 'rgb(255, 159, 64)',
		yellow: 'rgb(255, 205, 86)',
		green: 'rgb(75, 192, 192)',
		blue: 'rgb(54, 162, 235)',
		purple: 'rgb(153, 102, 255)',
		grey: 'rgb(201, 203, 207)'
	};
	datasets: any[] = [
		{
			label: 'Dataset 1 (linear interpolation)',
			backgroundColor: 'rgb(153, 102, 255)',
			borderColor: 'rgb(153, 102, 255)',
			fill: false,
			lineTension: 0,
			borderDash: [8, 4],
			data: []
		}
	];

	options: any = {
		type: 'line',
		scales: {

			xAxes: [{
				type: 'realtime',
				time: {
					stepSize: 10
				},
				realtime: {
					duration: 90000,
					refresh: 1000,
					delay: 2000,
					//onRefresh: '',

					// delay: 2000

				}

			}]

		}

	};
	subscription: Subscription;
	messages: Observable<Message>;
	cpId = '';
	currentUser: any;
	fileUrl: any;
	fileName = '';
	duration = '0';
	totalteleCount = 0;
	isConnected = false;
	fileToUpload: any = null;
	status;
	moduleName = "Add Device";
	parentDeviceObject: any = {};
	curentstaticsObject: any = {};
	//deviceObject = {};
	pipe = new DatePipe('en-US');
	deviceGuid = '';
	parentDeviceGuid = '';
	isEdit = false;
	deviceForm: FormGroup;
	checkSubmitStatus = false;
	teleLastReceivedData: Array<string> = [];
	templateList = [];
	greenhouseList = [];
	tagList = [];
	subscribed;
	stompConfiguration = {
		url: '',
		headers: {
			login: '',
			passcode: '',
			host: ''
		},
		heartbeat_in: 0,
		heartbeat_out: 2000,
		reconnect_delay: 5000,
		debug: true
	}
	statusList: StatusList[] = [
		{
			id: true,
			status: 'Active'
		},
		{
			id: false,
			status: 'In-active'
		}

	];
	ChartHead = ['Date/Time'];
	chartData = [];
	datadevice: any = [];
	columnArray: any = [];
	headFormate: any = {
		columns: this.columnArray,
		type: 'NumberFormat'
	};
	lineChartData = {
		chartType: 'LineChart',
		dataTable: this.chartData,
		options: {
			height: 430,
			interpolateNulls: true,
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
	};

	public lineChart: {
		chartType: 'LineChart',
		dataTable: [
			['Year', 'Sales', 'Expenses'],
			['2004', 1000, 400],
			['2005', 1170, 460],
			['2006', 660, 1120],
			['2007', 1030, 540]
		],
		options: { title: 'Company Performance' }
	};
	devicename: any;
	cId: any;


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
	constructor(
		private router: Router,
		private _notificationService: NotificationService,
		private activatedRoute: ActivatedRoute,
		private spinner: NgxSpinnerService,
		private deviceService: DeviceService,
		private gatewayService: GatewayService,
		private lookupService: LookupService,
		public _appConstant: AppConstant,
		private stompService: StompRService,
		private ruleService: RuleService,
		private dashboardService: DashboardService
	) {
		this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
		this.activatedRoute.params.subscribe(params => {
			this.parentDeviceGuid = params.parentDeviceGuid;
		});
		this.parentDeviceObject = { uniqueId: '', name: '', templateGuid: '', greenHouseGuid: '', note: '' };
		this.curentstaticsObject = {
			energyUsage: '',
			humidity: '',
			moisture: '',
			temperature: '',
			totalDevices: '',
			waterUsage: '',
			feedpressure: '',
			flowrate: '',
			intst: '',
		}
	}
	alerts = [];
	deviceConnected = false;
	// before view init
	ngOnInit() {

		this.createFormGroup();
		//this.getDeviceData();
		this.getAllDeviceData(this.parentDeviceGuid)
		//this.getDevicestatics();
		this.getSensorsLastValue();
		this.getChildDeviceList()
		this.getEnergyUsageChartData();
		this.getSoilnutritionChartData();
		this.getWaterConsumptionChartData();
		this.getAlertList();

	}

	getDevicestatics() {
		this.spinner.show();
		this.deviceService.getdevicestatics(this.parentDeviceGuid).subscribe(response => {
			if (response.isSuccess === true) {
				this.spinner.hide();
				this.curentstaticsObject = {
					energyUsage: (response.data.totalEnergyCount) ? response.data.totalEnergyCount : 0,
					humidity: (response.data.avgHumidity) ? response.data.avgHumidity : 0,
					moisture: (response.data.avgMoisture) ? response.data.avgMoisture : 0,
					temperature: (response.data.avgTemp) ? response.data.avgTemp : 0,
					totalDevices: (response.data.totalDevice) ? response.data.totalDevice : 0,
					waterUsage: (response.data.totalWaterUsage) ? response.data.totalWaterUsage : 0,
				}

			} else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}
	getSensorsLastValue() {
		this.spinner.show();
		this.deviceService.getSensorsLastValue(this.parentDeviceGuid).subscribe(response => {
			if (response.isSuccess === true) {
				response.data.forEach(element => {
					if (element.attributeName === "temp") {
						this.curentstaticsObject['temperature'] = element.attributeValue;
					} else if (element.attributeName === "moisture") {
						this.curentstaticsObject['moisture'] = element.attributeValue;
					} else if (element.attributeName === "humidity") {
						this.curentstaticsObject['humidity'] = element.attributeValue;
					} else if (element.attributeName === "feedpressure") {
						this.curentstaticsObject['feedpressure'] = element.attributeValue;
					} else if (element.attributeName === "flowrate") {
						this.curentstaticsObject['flowrate'] = element.attributeValue;
					} else if (element.attributeName === "intst") {
						this.curentstaticsObject['intst'] = element.attributeValue;
					}
				});
				this.spinner.hide();
				// this.curentstaticsObject = {
				// 	energyUsage: (response.data.totalEnergyCount) ? response.data.totalEnergyCount : 0,
				// 	humidity: (response.data.avgHumidity) ? response.data.avgHumidity : 0,
				// 	moisture: (response.data.avgMoisture) ? response.data.avgMoisture : 0,
				// 	temperature: (response.data.avgTemp) ? response.data.avgTemp : 0,
				// 	totalDevices: (response.data.totalDevice) ? response.data.totalDevice : 0,
				// 	waterUsage: (response.data.totalWaterUsage) ? response.data.totalWaterUsage : 0,
				// }

			} else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	getDeviceStatus(uniqueId) {
		this.spinner.show();
		this.deviceService.getDeviceStatus(uniqueId).subscribe(response => {
			if (response.isSuccess === true) {
				this.isConnected = response.data.isConnected;
				this.spinner.hide();
			} else {
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

	getAlertList() {
		let searchParameters = {
			pageNumber: 0,
			pageSize: 10,
			searchText: '',
			sortBy: 'eventDate desc',
			deviceGuid: this.parentDeviceGuid,
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
	getWaterConsumptionChartData() {
		let obj = { companyGuid: this.currentUser.userDetail.companyId, hardwareKitGuid: this.parentDeviceGuid };
		let data = []
		this.dashboardService.getWaterUsageChartData(obj).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['Months', 'Water Consumption'])
				}
				response.data.forEach(element => {
					data.push([element.month, parseFloat(element.value)]);
				});
				this.createHistoryChart('waterConsumption', data, 'Months', 'gal');
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
		let obj = { companyGuid: this.currentUser.userDetail.companyId, hardwareKitGuid: this.parentDeviceGuid };
		let data = []
		this.dashboardService.getEnergyUsageChartData(obj).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['Months', 'Energy'])
				}
				response.data.forEach(element => {
					data.push([element.month, parseFloat(element.value)]);
				});
				this.createHistoryChart('energyConsumption', data, 'Months', 'KWH');
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
		let obj = { companyGuid: this.currentUser.userDetail.companyId, hardwareKitGuid: this.parentDeviceGuid };
		let data = []
		this.dashboardService.getSoilnutritionChartData(obj).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				if (response.data.length) {
					data.push(['pH Level', 'N', 'P', 'K'])
				}
				response.data.forEach(element => {
					data.push([element.day, parseFloat(element['n']), parseFloat(element['p']), parseFloat(element['k'])]);
				});
				this.createHistoryChart('soilNutritions', data, 'Days', '% pH Level');
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});


	}
	createHistoryChart(key, data, hAxisTitle, vAxisTitle) {
		let height = this.chartHeight;
		let legend = { position: 'none' };
		var hAxis = {};
		if (key === 'soilNutritions') {
			height = 480;
			legend = { position: 'right' };
			hAxis = {
				title: hAxisTitle,
				gridlines: {
					count: 5
				},
				// slantedText:true,
				// slantedTextAngle:45,
			}
		} else {
			hAxis = {
				title: hAxisTitle,
				gridlines: {
					count: 5
				},
				slantedText: true,
				slantedTextAngle: 45,
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

	/**
	 * Get all the data related to parent device using forkjoin (Combine services)
	 * 
	 * @param deviceGuid 
	 * 
	 */
	getDeviceData() {
		this.spinner.show();
		this.deviceService.getDetailsdevice(this.parentDeviceGuid).subscribe(response => {
			if (response.isSuccess === true) {
				this.spinner.hide();
				this.parentDeviceObject = response.data;
				this.deviceService.getdevicesensor(this.cId, this.parentDeviceObject.templateGuid).subscribe(response => {
					if (response.isSuccess === true) {
						this.spinner.hide();
						let temp = [];
						response.data.forEach((element, i) => {
							console.log(element.text);
							var colorNames = Object.keys(this.chartColors);
							var colorName = colorNames[i % colorNames.length];
							var newColor = this.chartColors[colorName];
							var graphLabel = {
								label: element.text,
								backgroundColor: 'rgb(153, 102, 255)',
								borderColor: newColor,
								fill: false,
								cubicInterpolationMode: 'monotone',
								data: []
							}
							temp.push(graphLabel);

							console.log("graphLabel", newColor);
						});
						// response.data.forEach(element, i) => {

						// });
						this.datasets = temp;
					} else {
						this._notificationService.add(new Notification('error', response.message));
					}
				}, error => {
					this.spinner.hide();
					this._notificationService.add(new Notification('error', error));
				});
				this.getStompConfig();
			} else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}
	createChart() {
		let title = 'Date/Time(DD-MM HH:mm:ss)';
		if (this.duration !== '0') {
			title = 'Date/Time';
		}
		this.lineChartData = {
			chartType: 'LineChart',
			dataTable: this.chartData,
			options: {
				height: 430,
				interpolateNulls: true,
				hAxis: {
					title: title,
					gridlines: {
						count: 5
					},

				},
				vAxis: {
					title: 'Values',
					gridlines: {
						count: 5
					},
				}
			},
			formatters: this.headFormate
		};
	}

	getStompConfig() {
		this.ruleService.getStompConfig('LiveData').subscribe(response => {
			if (response.isSuccess) {
				this.stompConfiguration.url = response.data.url;
				this.stompConfiguration.headers.login = response.data.user;
				this.stompConfiguration.headers.passcode = response.data.password;
				this.stompConfiguration.headers.host = response.data.vhost;
				this.cpId = response.data.cpId;
				this.initStomp();
			}
		});
	}
	initStomp() {
		let config = this.stompConfiguration;
		this.stompService.config = config;
		this.stompService.initAndConnect();
		this.stompSubscribe();
	}
	public stompSubscribe() {
		/*if (this.subscribed) {
			return;
		}*/
		this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.parentDeviceObject.uniqueId);
		this.subscription = this.messages.subscribe(this.on_next);

		this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.parentDeviceObject.uniqueId + '-' + this.devicename);
		this.subscription = this.messages.subscribe(this.on_next);
		this.subscribed = true;
	}
	public on_next = (message: Message) => {
		let obj: any = JSON.parse(message.body);
		if (obj.data.msgType === 'telemetry') {
			let reporting_data = obj.data.data.reporting
			this.isConnected = true;
			let dates = obj.data.data.time;
			let now = moment();
			if (obj.data.data.status != 'off' && obj.data.data.status != 'on') {
				this.options = {
					type: 'line',
					scales: {

						xAxes: [{
							type: 'realtime',
							time: {
								stepSize: 10
							},
							realtime: {
								duration: 90000,
								refresh: 7000,
								delay: 2000,
								onRefresh: function (chart: any) {
									const getNestedObject = (nestedObj, pathArr) => {
										return pathArr.reduce((obj, key) =>
											(obj && obj[key] !== 'undefined') ? obj[key] : undefined, nestedObj);
									}
									if (obj.data.data.status != 'on') {
										chart.data.datasets.forEach(function (dataset: any) {
											var labeldata = dataset.label
											var kbc = getNestedObject(reporting_data, labeldata.split("."));
											dataset.data.push({

												x: now,

												y: kbc

											});

										});
									}
								}

							}
						}]
					}
				};
			}
			obj.data.data.time = now;
		} else if (obj.data.msgType === 'simulator' || obj.data.msgType === 'device') {
			if (obj.data.data.status === 'off') {
				this.isConnected = false;
			} else {
				this.isConnected = true;
			}
		}
		/*var colorNames = Object.keys(this.chartColors);
		var colorName = colorNames[this.datasets.length % colorNames.length];
		var newColor = this.chartColors[colorName];
		var test = {
			label: 'Dataset 3 (cubic interpolation)',
			backgroundColor: 'rgb(153, 102, 255)',
			borderColor: newColor,
			fill: false,
			cubicInterpolationMode: 'monotone',
			data: []
		}
		console.log("datssshiii",this.datasets.push(test)) */


	}
	createFormGroup() {
		this.deviceForm = new FormGroup({
			uniqueId: new FormControl('', [Validators.required]),
			name: new FormControl('', [Validators.required]),
			type: new FormControl(0),
			parentDeviceGuid: new FormControl((this.parentDeviceGuid) ? this.parentDeviceGuid : null),
			tag: new FormControl('', [Validators.required]),
			note: new FormControl(''),
			companyGuid: new FormControl(null),
			greenHouseGuid: new FormControl(Guid.EMPTY),
			templateGuid: new FormControl(Guid.EMPTY),
			isProvisioned: new FormControl(false),
			isActive: new FormControl(true)
		});
	}

	/**
	 * Get all the data related to parent device using forkjoin (Combine services)
	 * 
	 * @param deviceGuid 
	 * 
	 */
	getAllDeviceData(deviceGuid) {

		this.spinner.show();
		let getParentDeviceDetails = this.gatewayService.getgatewayDetails(deviceGuid);



		let getGreenHouseLookup = this.gatewayService.getGreenHouseLookup();
		let getTemplateLookup = this.gatewayService.getTemplateLookup();
		// capture response until all the APIs got success
		forkJoin([getParentDeviceDetails, getGreenHouseLookup, getTemplateLookup])
			.subscribe(response => {
				this.setParentDeviceDetails(response[0]);
				if (response[0].data['uniqueId']) {
					this.getDeviceStatus(response[0].data['uniqueId']);
				}
				this.setGreenHouseLookup(response[1]);
				this.setTemplateLookup(response[2]);
				this.spinner.hide();
			}, error => {
				this.spinner.hide();
				this._notificationService.add(new Notification('error', error));
			});

	}






	/**
	 * set parent device details
	 * @param response 
	 */
	setParentDeviceDetails(response) {
		if (response.isSuccess === true) {
			this.parentDeviceObject = response.data;
			//Get tags lookup once parent device data is fetched
			this.getTagsLookup();
		} else {
			this._notificationService.add(new Notification('error', response.message));
		}

	}
	/**
	 * set greenhouse lookup
	 * @param response 
	*/
	setGreenHouseLookup(response) {
		if (response.isSuccess === true) {
			this.greenhouseList = response['data'];
		} else {
			this._notificationService.add(new Notification('error', response.message));
		}
	}

	/**
	 * set template lookup
	 * only gateway supported template
	 *  @param response 
	 */
	setTemplateLookup(response) {
		if (response.isSuccess === true) {
			this.templateList = response['data'];
		} else {
			this._notificationService.add(new Notification('error', response.message));
		}
	}

	/**
	 * Get tags lookup once parent device data is fetched
	 */
	getTagsLookup() {

		if (this.parentDeviceObject) {
			this.lookupService.getTagsLookup(this.parentDeviceObject.templateGuid).
				subscribe(response => {
					if (response.isSuccess === true) {
						this.tagList = response['data'];
					} else {
						this._notificationService.add(new Notification('error', response.message));
					}
				}, error => {
					this.spinner.hide();
					this._notificationService.add(new Notification('error', error));
				})
		}

	}

	log(obj) {

	}


	/**
	 * Find a value from the look up data
	 * 
	 * @param obj 
	 * 
	 * @param findByvalue 
	 * 
	 */
	getIndexByValue(obj, findByvalue) {
		let index = obj.findIndex(
			(tmpl) => { return (tmpl.value == findByvalue.toUpperCase()) }
		);
		if (index > -1) return obj[index].text;
		return;
	}


	/**
	 * Add device under gateway
	 * only gateway supported device
	 */
	addChildDevice() {
		this.checkSubmitStatus = true;
		if (this.deviceForm.status === "VALID") {
			this.spinner.show();
			// overwrite default values
			this.deviceForm.patchValue({ 'companyGuid': this.currentUser.userDetail.companyId });
			(this.deviceForm.value.note == "" || typeof this.deviceForm.value.note === "undefined")
				? this.deviceForm.patchValue({ 'note': "" })
				: "";

			let successMessage = this._appConstant.msgCreated.replace("modulename", "Device");
			this.deviceService.addUpdateDevice(this.deviceForm.value).subscribe(response => {
				this.spinner.hide();
				if (response.isSuccess === true) {
					this.deviceForm.reset();
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

	randomScalingFactor() {
		return (Math.random() > 0.5 ? 1.0 : -1.0) * Math.round(Math.random() * 100);
	}
	getChildDeviceList() {

		if (!this.parentDeviceGuid)
			this._notificationService.add(new Notification('error', "Parent device ID is not found"));

		this.spinner.show();
		this.deviceService.getChildDevices(this.parentDeviceGuid, this.searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true && response.data.items != '') {
				//this.totalRecords = response.data.count;
				this.datadevice = response.data.items;
				this.devicename = response.data.items[0].uniqueId
				this.cId = response.data.items[0].guid
				this.getDeviceData();
			}
			else {
				//this._notificationService.add(new Notification('error', response.message));
				//this.dataSource = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}
	getchilddevice(chiddeviceId) {
		let obj = this.datadevice.find(o => o.guid === chiddeviceId);
		this.devicename = obj.uniqueId;
		this.spinner.show();
		this.deviceService.getdevicesensor(chiddeviceId, this.parentDeviceObject.templateGuid).subscribe(response => {
			if (response.isSuccess === true) {
				this.spinner.hide();
				let temp = [];
				response.data.forEach((element, i) => {
					//console.log(element.text);
					var colorNames = Object.keys(this.chartColors);
					var colorName = colorNames[i % colorNames.length];
					var newColor = this.chartColors[colorName];
					var graphLabel = {
						label: element.text,
						backgroundColor: 'rgb(153, 102, 255)',
						borderColor: newColor,
						fill: false,
						cubicInterpolationMode: 'monotone',
						data: []
					}
					temp.push(graphLabel);

					//console.log("graphLabel", newColor);
				});
				// response.data.forEach(element, i) => {

				// });
				this.datasets = temp;
				//this.getStompConfig();
				//this.stompSubscribe()
				this.subscription.unsubscribe();
				//this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.sensorNamedata);
				this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.parentDeviceObject.uniqueId + '-' + this.devicename);
				this.subscription = this.messages.subscribe(this.on_next);
				this.subscribed = true;
			} else {
				this._notificationService.add(new Notification('error', response.message));
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}
}
