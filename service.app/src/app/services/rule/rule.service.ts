import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'

import { ApiConfigService,NotificationService } from 'app/services';



@Injectable({
	providedIn: 'root'
})
export class RuleService {
	protected apiServer = ApiConfigService.settings.apiServer;
	constructor(
		private httpClient: HttpClient,
		private _notificationService: NotificationService) {
			this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
		}

	getSeveritylevelLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/severitylevel').map(response => {
			return response;
		});
	}

	getConditionLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/condition').map(response => {
			return response;
		});
	}

	getUsersLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/iotuser').map(response => {
			return response;
		});
	}

	getDeviceLookup(templateGuid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/iotdevice/' + templateGuid).map(response => {
			return response;
		});
	}

	getEntityLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/iotentity').map(response => {
			return response;
		});
	}

	getGreenHouseLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/greenhouse').map(response => {
			return response;
		});
	}

	getRoleLookup() {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/role').map(response => {
			return response;
		});
	}

	getRuleList(serachText) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminrule/search?searchText=' + serachText).map(response => {
			return response;
		});
	}

	deleteRule(guid) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/adminrule/delete/' + guid, '').map(response => {
			return response;
		});
	}

	updateRuleStatus(guid, status) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/adminrule/updateStatus/' + guid + '/' + status, '').map(response => {
			return response;
		});
	}

	manageRule(postData) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/adminrule/manage', postData).map(response => {
			return response;
		});
	}

	getRuleDetail(guid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminrule/' + guid).map(response => {
			return response;
		});
	}

	getUserRuleList(serachText) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/notifications/search?searchText=' + serachText).map(response => {
			return response;
		});
	}

	updateUserRuleStatus(guid, status) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/notifications/updateStatus/' + guid + '/' + status, '').map(response => {
			return response;
		});
	}

	manageUserRule(postData) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/notifications/manage', postData).map(response => {
			return response;
		});
	}

	deleteUserRule(guid) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/notifications/delete/' + guid, '').map(response => {
			return response;
		});
	}

	getUserRuleDetail(guid) {
		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/notifications/' + guid).map(response => {
			return response;
		});
	}

	verifyCondtion(postData) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/notifications/verify', postData).map(response => {
			return response;
		});
	}

	//key : UIAlert || LiveData
	getStompConfig(key) {
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/configuration/'+key,'').map(response => {
			return response;
		});
	}
}
