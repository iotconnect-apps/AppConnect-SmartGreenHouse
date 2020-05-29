import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService, NotificationService } from 'app/services';


@Injectable({
	providedIn: 'root'
})
export class GreenHouseService {
	protected apiServer = ApiConfigService.settings.apiServer;

	cookieName = 'FM';
	constructor(private cookieService: CookieService,
		private httpClient: HttpClient,
		private _notificationService: NotificationService) {
		this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
	}


	getgreenHouse(parameters) {

		const reqParameter = {
			params: {
				'pageNo': parameters.pageNumber,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/greenhouse/search', reqParameter).map(response => {
			return response;
		});
	}

	getgreenhouseDetails(greenHouseGuid) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/greenhouse/' + greenHouseGuid).map(response => {
			return response;
		});
	}

	deletegreenHouse(greenHouseGuid) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/greenhouse/delete/' + greenHouseGuid, "").map(response => {
			return response;
		});
	}

	getcountryList() {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/country').map(response => {
			return response;
		});
	}
	getstateList(countryId) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/state/' + countryId).map(response => {
			return response;
		});
	}
	removeGHImage(entityId) {
		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/greenhouse/deleteimage/'+entityId,{}).map(response => {
		  return response;
		});
	  }
	addGreenhouse(data) {

		const formData = new FormData();
		for (const key of Object.keys(data)) {
			const value = data[key];
			if (data[key])
				formData.append(key, value);
		}

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/greenhouse/manage', formData).map(response => {
			return response;
		});
	}

	changeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/greenhouse/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
	}

	getDeviceList(greenHouseId) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/getgreenhousedevices/' + greenHouseId).map(response => {
			return response;
		});
	}

}
