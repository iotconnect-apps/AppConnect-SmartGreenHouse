import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'

import { environment } from './../../../environments/environment'

@Injectable({
	providedIn: 'root'
})

export class SettingsService {
	cookieName = 'FM';

	constructor(
		private cookieService: CookieService,
		private httpClient: HttpClient
	) { }

	getConfigurationData() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/getConfigurationData', configHeader).map(response => {
			return response;
		});
	}

	getOfficeSignInUrl() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/getOfficeSignInUrl', configHeader).map(response => {
			return response;
		});
	}

	unRegisterOffice() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/unRegisterOffice', configHeader).map(response => {
			return response;
		});
	}

	getGoogleSignInUrl() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/getGoogleSignInUrl', configHeader).map(response => {
			return response;
		});
	}

	unRegisterGoogle() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/unRegisterGoogle', configHeader).map(response => {
			return response;
		});
	}

	getSettingDetails() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/getSettingDetails', configHeader).map(response => {
			return response;
		});
	}

	apiIntegrationInfo() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(environment.baseUrl + 'settings/apiIntegrationInfo', configHeader).map(response => {
			return response;
		});
	}

	createMeetingInOffice(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'settings/createMeetingInOffice', data, configHeader).map(response => {
			return response;
		});
	}

	createMeetingInGoogle(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'settings/createMeetingInGoogle', data, configHeader).map(response => {
			return response;
		});
	}

	deleteMeetingInGoogle(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'settings/deleteMeetingInGoogle', data, configHeader).map(response => {
			return response;
		});
	}

	deleteMeetingInOffice(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'settings/deleteMeetingInOffice', data, configHeader).map(response => {
			return response;
		});
	}

	syncGoogleEvents(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'settings/syncGoogleEvents', data, configHeader).map(response => {
			return response;
		});
	}

	syncOffice365Events(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(environment.baseUrl + 'settings/syncOffice365Events', data, configHeader).map(response => {
			return response;
		});
	}
}