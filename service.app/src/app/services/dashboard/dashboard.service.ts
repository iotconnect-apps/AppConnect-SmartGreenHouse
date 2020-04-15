import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService ,NotificationService} from 'app/services';
@Injectable()

export class DashboardService {
	protected apiServer = ApiConfigService.settings.apiServer;
	cookieName = 'FM';
	//protected apiServer = AppConfigService.settings.apiServer;
	constructor(
		private http: HttpClient,
		private _notificationService: NotificationService,
		private cookieService: CookieService
	) { 
		this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
	}

	getDashboardoverview() {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		return this.http.get<any>(this.apiServer.baseUrl + 'api/dashboard/overview/' + currentUser.userDetail.companyId).map(response => {
			return response;
		});
	}

  getActiveGreenHouse() {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/greenhouse?isActive=true').map(response => {
      return response;
    });
  }

	getGreenHouse() {
		return this.http.get<any>(this.apiServer.baseUrl + 'api/greenhouse').map(response => {
			return response;
		});
	}

	getGreenHouseAlert() {
		return this.http.get<any>(this.apiServer.baseUrl + 'api/greenhouse/alerts').map(response => {
			return response;
		});
	}

	getAlertsList(parameters) {
		var configHeader = {
		  headers: {
			'Content-Type': 'application/json',
		  }
		};
	
		const parameter = {
		  params: {
			'pageNo': parameters.pageNumber + 1,
			'pageSize': parameters.pageSize,
			'searchText': parameters.searchText,
			'orderBy': parameters.sortBy,
			'deviceGuid': parameters.deviceGuid,
			'entityGuid': parameters.entityGuid,
		  },
		  timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);
	
		return this.http.get<any>(this.apiServer.baseUrl + 'api/alert', reqParameter).map(response => {
		  return response;
		});
	  }
	

	getGreenHouseDetail(greenhouseguid) {
		return this.http.get<any>(this.apiServer.baseUrl + 'api/dashboard/getgreenhousedetail/' + greenhouseguid).map(response => {
			return response;
		});
	}
	getSoilnutrition(greenhouseguid) {
		return this.http.get<any>(this.apiServer.baseUrl + 'api/greenhouse/getsoilnutrition/' + greenhouseguid).map(response => {
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

		return this.http.get(this.apiServer.baseUrl + 'api/dashboard/notification', configHeader).map(response => {
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

		return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckUsage', configHeader).map(response => {
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

		return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckActivity', configHeader).map(response => {
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

		return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckGraph', configHeader).map(response => {
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

		return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getStompConfiguration', configHeader).map(response => {
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

		return this.http.post(this.apiServer.baseUrl + 'api/dashboard/getDeviceAttributeHistoricalData', data, configHeader).map(response => {
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

		return this.http.post(this.apiServer.baseUrl + 'api/dashboard/getDeviceAttributes', data, configHeader).map(response => {
			return response;
		});
	}

	tripStatus(id, data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.put<any>(this.apiServer.baseUrl + 'api/trip/' + id + '/status', data, configHeader).map(response => {
			return response;
		});
	}

	startSimulator(id, isSalesTemplate = true) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.http.get<any>(this.apiServer.baseUrl + 'api/trip/startSimulator/' + id + '/' + isSalesTemplate, configHeader).map(response => {
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

		return this.http.get(this.apiServer.baseUrl + 'api/truck/lookup', configHeader).map(response => {
			return response;
		});
	}

	// Get getWaterUsageChartData
	getWaterUsageChartData(data) {
		return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getwaterusage', data).map(response => {
			return response;
		});
	}

	// Get Gateway Count
	getEnergyUsageChartData(data) {
		return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getenergyusage', data).map(response => {
			return response;
		});
	}

	// Get Gateway Count
	getSoilnutritionChartData(data) {
		return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getsoilnutrition', data).map(response => {
			return response;
		});
	}

	// Convert timestamp ti since time
	timeSince(date) {
		date = new Date(date)
		let minute = 60;
		let hour = minute * 60;
		let day = hour * 24;
		let month = day * 30;
		let year = day * 365;

		let suffix = ' ago';

		let elapsed = Math.floor((Date.now() - date) / 1000);

		if (elapsed < minute) {
			return 'just now';
		}

		// get an array in the form of [number, string]
		let a = elapsed < hour && [Math.floor(elapsed / minute), 'minute'] ||
			elapsed < day && [Math.floor(elapsed / hour), 'hour'] ||
			elapsed < month && [Math.floor(elapsed / day), 'day'] ||
			elapsed < year && [Math.floor(elapsed / month), 'month'] ||
			[Math.floor(elapsed / year), 'year'];

		// pluralise and append suffix
		return a[0] + ' ' + a[1] + (a[0] === 1 ? '' : 's') + suffix;
	}


}
