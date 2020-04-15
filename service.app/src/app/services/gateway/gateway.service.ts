import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { ApiConfigService,NotificationService } from 'app/services';





@Injectable({
	providedIn: 'root'
})


export class GatewayService {
	protected apiServer = ApiConfigService.settings.apiServer;
	constructor(
		private httpClient: HttpClient,
		private _notificationService: NotificationService) {
			this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
		}

	// gateway supported template lookup
	getTemplateLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/template').map(response => {
			return response;
		});
	}

	// gateway supported template lookup
	getTemplateIotLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/alltemplateiot').map(response => {
			return response;
		});
	}

	getAllTemplateLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/alltemplate').map(response => {
			return response;
		});
	}
	getTemplateAttribueLookup(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/attributes/' + templateGuid).map(response => {
			return response;
		});
	}

	getTemplateAttribueIotLookup(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/allattributesiot/' + templateGuid).map(response => {
			return response;
		});
	}
	getKitAttribueLookup(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/kitype/attributes/' + templateGuid).map(response => {
			return response;
		});
	}

	getTemplateCommandLookup(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/commands/' + templateGuid).map(response => {
			return response;
		});
	}

	getTemplateCommandIotLookup(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/allcommandsiot/' + templateGuid).map(response => {
			return response;
		});
	}


	// GreenHouse lookup
	getGreenHouseLookup() {

		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/GreenHouse/' + currentUser.userDetail.companyId).map(response => {
			return response;
		});
	}

	// Add or update Gateway
	addUpdategateway(data) {

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/gateway/manage', data).map(response => {
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

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/gateway/search', reqParameter).map(response => {
			return response;
		});
	}

	changegatewayStatus(gatewayGuid, data) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/gateway/' + gatewayGuid + '/status', data).map(response => {
			return response;
		});
	}

	deletegateway(gatewayGuid) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/gateway/delete/' + gatewayGuid, {}).map(response => {
			return response;
		});
	}

	uploadPicture(gatewayGuid, file) {

		const data = new FormData();
		data.append('image', file);

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/gateway/' + gatewayGuid + '/image', data).map(response => {
			return response;
		});
	}

	getgatewayDetails(gatewayGuid) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/gateway/' + gatewayGuid, configHeader).map(response => {
			return response;
		});
	}

	updategateway(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/gateway/manage', data, configHeader).map(response => {
			return response;
		});
	}

	changeStatus(gatewayId, isActive) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/gateway/updatestatus/' + gatewayId + '/' + isActive, {}).map(response => {
			return response;
		});
	}

	// Get Gateway Count
	getGatewayCount(data) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/validatekit/' + data).map(response => {
			return response;
		});
	}

	// Provision Kit
	provisionKit(data) {

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/device/provisionkit', data).map(response => {
			return response;
		});
	}
}
