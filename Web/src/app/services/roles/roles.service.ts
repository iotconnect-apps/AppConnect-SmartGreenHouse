import 'rxjs/add/operator/map'


import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'

import { environment } from './../../../environments/environment'


@Injectable({
	providedIn: 'root'
})

export class RolesService {

	constructor(
		private httpClient: HttpClient

	) { }


	getRoles(parameters) {
		const parameter = {
			params: {
				'pageNo': parameters.pageNumber + 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'role/search', parameter).map(response => {
			return response;
		});
	}

	addUpdateRole(data) {

		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		const appendParams = {
			"solutionGuid" : currentUser.userDetail.solutionGuid
		};

		var reqParameter = Object.assign(data, appendParams);
		return this.httpClient.post<any>(environment.baseUrl + 'role/manage', reqParameter).map(response => {
			return response;
		});
	}

	getRoleDetails(roleId){
		
		return this.httpClient.get<any>(environment.baseUrl + 'role/'+roleId).map(response => {
			return response;
		});
	}

	deleteRole(roleId){
		
		return this.httpClient.put<any>(environment.baseUrl + 'role/delete/'+roleId, {}).map(response => {
			return response;
		});
	}

	changeStatus(roleId, isActive){
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(environment.baseUrl + 'role/updatestatus/'+roleId+'/'+status, {}).map(response => {
			return response;
		});
	}




}
