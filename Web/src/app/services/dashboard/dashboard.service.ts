import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'

import { environment } from './../../../environments/environment'

@Injectable()

export class DashboardService {
	cookieName = 'FM';

	constructor(
		private http: HttpClient,
		private cookieService: CookieService
	) { }

	getDashboardoverview() {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		return this.http.get<any>(environment.baseUrl + 'dashboard/overview/' + currentUser.userDetail.companyId).map(response => {
			return response;
		});
	}

	getGreenHouse() {
		return this.http.get<any>(environment.baseUrl + 'greenhouse').map(response => {
			return response;
		});
	}

	getGreenHouseDetail(greenhouseguid) {
		return this.http.get<any>(environment.baseUrl + 'dashboard/getgreenhousedetail/' + greenhouseguid).map(response => {
			return response;
		});
	}
	getSoilnutrition(greenhouseguid) {
		return this.http.get<any>(environment.baseUrl + 'greenhouse/getsoilnutrition/' + greenhouseguid).map(response => {
			return response;
		});
	}
	

	getNotificationList() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get(environment.baseUrl + 'dashboard/notification', configHeader).map(response => {
			return response;
		});
	}

	getTruckUsage() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get(environment.baseUrl + 'dashboard/getTruckUsage', configHeader).map(response => {
			return response;
		});
	}
	
	getTruckActivity() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get(environment.baseUrl + 'dashboard/getTruckActivity', configHeader).map(response => {
			return response;
		});
	}

	getTruckGraph() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get(environment.baseUrl + 'dashboard/getTruckGraph', configHeader).map(response => {
			return response;
		});
	}

	getStompCon() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get(environment.baseUrl + 'dashboard/getStompConfiguration', configHeader).map(response => {
			return response;
		});
	}
	
	getDeviceAttributeHistoricalData(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.post(environment.baseUrl + 'dashboard/getDeviceAttributeHistoricalData', data, configHeader).map(response => {
			return response;
		});
	}

	getSensors(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.post(environment.baseUrl + 'dashboard/getDeviceAttributes', data, configHeader).map(response => {
			return response;
		});
	}

	tripStatus(id,data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.put<any>(environment.baseUrl + 'trip/' + id +'/status', data, configHeader).map(response => {
			return response;
		});
	}

	startSimulator(id,isSalesTemplate = true) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get<any>(environment.baseUrl + 'trip/startSimulator/'+id+'/'+isSalesTemplate, configHeader).map(response => {
			return response;
		});
	}

	getTruckLookup() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get(environment.baseUrl + 'truck/lookup', configHeader).map(response => {
			return response;
		});
	}
}