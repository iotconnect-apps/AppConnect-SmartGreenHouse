import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'

import { environment } from './../../../environments/environment'
@Injectable({
	providedIn: 'root'
})

export class UserService {

	cookieName = 'FM';
	constructor(private cookieService: CookieService,
		private httpClient: HttpClient) { }

	getUserlist(parameters) {
		const parameter = {
			params: {
				'pageNo': parameters.pageNumber + 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};

		return this.httpClient.get<any>(environment.baseUrl + 'user/search', parameter).map(response => {
			return response;
		});
	}

	getUserDetails(userGuid) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		let isAdmin = currentUser.userDetail.isAdmin;
		if(isAdmin){
			return this.httpClient.get<any>(environment.baseUrl + 'adminuser/' + userGuid).map(response => {
				return response;
			});
		}else{
			return this.httpClient.get<any>(environment.baseUrl + 'user/' + userGuid).map(response => {
					return response;
				});
		}
       
	
	}

	deleteUser(userGuid) {

		return this.httpClient.put<any>(environment.baseUrl + 'user/delete/' + userGuid, "").map(response => {
			return response;
		});
	}
	// login
	login(data) {

		return this.httpClient.post<any>(environment.baseUrl + 'account/login', data).map(response => {
			if (response.isSuccess === true && response.data.access_token) {
				// store user details and jwt token in the local storage to keep the user logged in between page refreshes
				localStorage.setItem('currentUser', JSON.stringify(response.data));
			}
			return response;
		});

	}

	// logout
	logout() {
		// remove user from the local storage
		localStorage.removeItem('currentUser');
	}

	getroleList() {

    return this.httpClient.get<any>(environment.baseUrl + 'role/company').map(response => {
			return response;
		});
	}

	getTimezoneList() {


		return this.httpClient.get<any>(environment.baseUrl + 'Lookup/timezone').map(response => {
			return response;
		});
	}

	addUser(data) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		let isAdmin = currentUser.userDetail.isAdmin;
		if(isAdmin){
			return this.httpClient.post<any>(environment.baseUrl + 'adminuser/manage', data).map(response => {
				return response;
			});
		}else{
			return this.httpClient.post<any>(environment.baseUrl + 'user/manage', data).map(response => {
				return response;
			});
		}
       
		
	}

	changeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(environment.baseUrl + 'user/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
	}

	/**
	 * Change password of a user
	*/
	changePassword(data) {
		return this.httpClient.post<any>(environment.baseUrl + 'account/changepassword', data).map(response => {
			return response;
		});
	}
	// adminlogin
	loginadmin(data) {

		return this.httpClient.post<any>(environment.baseUrl + 'account/adminlogin', data).map(response => {
			if (response.isSuccess === true && response.data.access_token) {
				// store user details and jwt token in the local storage to keep the user logged in between page refreshes
				localStorage.setItem('currentUser', JSON.stringify(response.data));
			}
			return response;
		});

	}
	addadminUser(data) {

		return this.httpClient.post<any>(environment.baseUrl + 'adminuser/manage', data).map(response => {
			return response;
		});
	}
	getadminUserDetails(userGuid) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'adminuser/' + userGuid, configHeader).map(response => {
			return response;
		});
	}
	deleteadminUser(userGuid) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.put<any>(environment.baseUrl + 'adminuser/delete/' + userGuid, configHeader).map(response => {
			return response;
		});
	}
	adminchangeStatus(id, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.put<any>(environment.baseUrl + 'adminuser/updatestatus/' + id + '/' + status, {}).map(response => {
			return response;
		});
	}
	getAdminUserlist(parameters) {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
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
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(environment.baseUrl + 'adminuser/list', reqParameter).map(response => {
			return response;
		});
	}
	
}
