import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'

import { environment } from './../../../environments/environment'

@Injectable({
	providedIn: 'root'
})

export class GatewayService {

	constructor(
		private httpClient: HttpClient
	) { }

	// gateway supported template lookup
	getTemplateLookup() {
		return this.httpClient.get<any>(environment.baseUrl + 'lookup/template').map(response => {
			return response;
		});
	}

	getTemplateAttribueLookup(templateGuid) {
		return this.httpClient.get<any>(environment.baseUrl + 'lookup/attributes/'+templateGuid).map(response => {
			return response;
		});
	}

	getKitAttribueLookup(templateGuid) {
		return this.httpClient.get<any>(environment.baseUrl + 'kitype/attributes/'+templateGuid).map(response => {
			return response;
		});
	}

	getTemplateCommandLookup(templateGuid) {
		return this.httpClient.get<any>(environment.baseUrl + 'lookup/commands/'+templateGuid).map(response => {
			return response;
		});
	}

	// GreenHouse lookup
	getGreenHouseLookup() {

		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		return this.httpClient.get<any>(environment.baseUrl + 'lookup/GreenHouse/' + currentUser.userDetail.companyId).map(response => {
			return response;
		});
	}

	// Add or update Gateway
	addUpdategateway(data) {

		return this.httpClient.post<any>(environment.baseUrl + 'gateway/manage', data).map(response => {
			return response;
		});
	}

	getgateways(parameters) {

		const reqParameter = {
			params: {
				'pageNo': parameters.pageNumber + 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'gateway/search', reqParameter).map(response => {
			return response;
		});
	}

	changegatewayStatus(gatewayGuid, data) {

		return this.httpClient.put<any>(environment.baseUrl + 'gateway/' + gatewayGuid + '/status', data).map(response => {
			return response;
		});
	}

	deletegateway(gatewayGuid) {

		return this.httpClient.put<any>(environment.baseUrl + 'gateway/delete/' + gatewayGuid, {}).map(response => {
			return response;
		});
	}

	uploadPicture(gatewayGuid, file) {

		const data = new FormData();
		data.append('image', file);

		return this.httpClient.put<any>(environment.baseUrl + 'gateway/' + gatewayGuid + '/image', data).map(response => {
			return response;
		});
	}

	getgatewayDetails(gatewayGuid) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'gateway/' + gatewayGuid, configHeader).map(response => {
			return response;
		});
	}

	updategateway(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'gateway/manage', data, configHeader).map(response => {
			return response;
		});
	}

	changeStatus(gatewayId, isActive) {
    return this.httpClient.post<any>(environment.baseUrl + 'gateway/updatestatus/' + gatewayId + '/' + isActive, {}).map(response => {
			return response;
		});
  }

  // Get Gateway Count
  getGatewayCount(data) {
   
    return this.httpClient.get<any>(environment.baseUrl + 'device/validatekit/' + data).map(response => {
      return response;
    });
  }

  // Provision Kit
  provisionKit(data) {

    return this.httpClient.post<any>(environment.baseUrl + 'device/provisionkit', data).map(response => {
      return response;
    });
  }
}
