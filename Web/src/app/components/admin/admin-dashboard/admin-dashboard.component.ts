import * as moment from 'moment-timezone'
import { Component, OnInit } from '@angular/core'
import { NgxSpinnerService } from 'ngx-spinner'
import { DashboardService, NotificationService, Notification } from '../../../services';

@Component({
	selector: 'app-admin-dashboard',
	templateUrl: './admin-dashboard.component.html',
	styleUrls: ['./admin-dashboard.component.css'],
})

export class AdminDashboardComponent implements OnInit {
  lat = 23.033863;
  lng = 72.585022;
  isShowLeftMenu = true;
  greenhouse = [];
  energyUsage: any;
  humidity: any;
  moisture: any;
  temperature: any;
  totalDevices: any;
  waterUsage: any;
  totalGreenhouse: any;
  totalCorp: any;
  totalConnectedDevices: any;
  totalDisconnectedDevices: any;

	constructor(
    private spinner: NgxSpinnerService,
    public dashboardService: DashboardService,
    private _notificationService: NotificationService
	) { }

  ngOnInit() {
    this.getDashbourdCount()
    this.getGreenhouse()
  }

  getDashbourdCount() {
    this.spinner.show();
    this.dashboardService.getDashboardoverview().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.totalGreenhouse = response.data.totalGreenhouse
        this.totalCorp = response.data.totalCorp
        this.totalConnectedDevices = response.data.totalConnectedDevices
        this.totalDisconnectedDevices = response.data.totalDisconnectedDevices

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
    this.dashboardService.getGreenHouse().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {

        this.greenhouse = response.data;
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
        this.dashboardService.getSoilnutrition(greenhouseguid).subscribe(response => {
        })
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
