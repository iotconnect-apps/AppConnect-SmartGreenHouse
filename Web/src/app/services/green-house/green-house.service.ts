import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'

import { environment } from '@environment/environment'
@Injectable({
	providedIn: 'root'
})
export class GreenHouseService {
	cookieName = 'FM';
	constructor(private cookieService: CookieService,
		private httpClient: HttpClient) { }

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

		return this.httpClient.get<any>(environment.baseUrl + 'greenhouse/search', reqParameter).map(response => {
			return response;
		});
	}

	getgreenhouseDetails(greenHouseGuid) {

		return this.httpClient.get<any>(environment.baseUrl + 'greenhouse/' + greenHouseGuid).map(response => {
			return response;
		});
	}

	deletegreenHouse(greenHouseGuid) {

		return this.httpClient.put<any>(environment.baseUrl + 'greenhouse/delete/' + greenHouseGuid, "").map(response => {
			return response;
		});
	}

	getcountryList() {

		return this.httpClient.get<any>(environment.baseUrl + 'lookup/country').map(response => {
			return response;
		});
	}
	getcitylist(countryId) {

		return this.httpClient.get<any>(environment.baseUrl + 'lookup/state/' + countryId).map(response => {
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

    return this.httpClient.post<any>(environment.baseUrl + 'greenhouse/manage', formData).map(response => {
			return response;
		});
	}

	changeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(environment.baseUrl + 'greenhouse/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
  }

  getDeviceList(greenHouseId) {

    return this.httpClient.get<any>(environment.baseUrl + 'device/getgreenhousedevices/' + greenHouseId).map(response => {
      return response;
    });
  }
  
}
