import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'

import { environment } from './../../../environments/environment'

@Injectable({
	providedIn: 'root'
})
export class RuleService {

	constructor(private httpClient: HttpClient) { }
	
	getSeveritylevelLookup() {
		return this.httpClient.get<any>(environment.baseUrl + 'lookup/severitylevel').map(response => {
			return response;
		});
	}

	getConditionLookup() {
		return this.httpClient.get<any>(environment.baseUrl + 'lookup/condition').map(response => {
			return response;
		});
	}

	getRuleList(serachText) {
		return this.httpClient.get<any>(environment.baseUrl + 'adminrule/search?searchText='+serachText).map(response => {
			return response;
		});
	}

	deleteRule(guid) {

		return this.httpClient.put<any>(environment.baseUrl + 'adminrule/delete/'+guid,'').map(response => {
			return response;
		});
	}

	updateRuleStatus(guid,status) {
		return this.httpClient.post<any>(environment.baseUrl + 'adminrule/updateStatus/'+guid+'/'+status,'').map(response => {
			return response;
		});
	}

	manageRule(postData) {
		return this.httpClient.post<any>(environment.baseUrl + 'adminrule/manage', postData).map(response => {
			return response;
		});
	}

	getRuleDetail(guid) {
		return this.httpClient.get<any>(environment.baseUrl + 'adminrule/'+guid).map(response => {
			return response;
		});
	}

	getUserRuleList(serachText) {
		return this.httpClient.get<any>(environment.baseUrl + 'rule?searchText='+serachText).map(response => {
			return response;
		});
	}

	updateUserRuleStatus(guid,status) {
		return this.httpClient.post<any>(environment.baseUrl + 'rule/updateStatus/'+guid+'/'+status,'').map(response => {
			return response;
		});
	}

	manageUserRule(postData) {
		return this.httpClient.post<any>(environment.baseUrl + 'rule/manage', postData).map(response => {
			return response;
		});
	}

	deleteUserRule(guid) {

		return this.httpClient.put<any>(environment.baseUrl + 'rule/delete/'+guid,'').map(response => {
			return response;
		});
	}

	getUserRuleDetail(guid) {
		return this.httpClient.get<any>(environment.baseUrl + 'rule/'+guid).map(response => {
			return response;
		});
	}
}
