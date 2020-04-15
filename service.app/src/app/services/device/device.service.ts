import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService,NotificationService } from 'app/services';




@Injectable({
	providedIn: 'root'
})

export class DeviceService {
	cookieName = 'FM';
	protected apiServer = ApiConfigService.settings.apiServer;
	constructor(
		private cookieService: CookieService,
		private httpClient: HttpClient,
		private _notificationService: NotificationService) {
			this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
		}

	getGatewayLookup() {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/gateway').map(response => {

			return response;
		});
	}

	 getsubcribesyncdata() {


	return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getlastsyncdetails').map(response => {
		return response;
	});
}

	getChildDevices(parentID, parameters) {

		const parameter = {
			params: {
				'parentDeviceGuid' : parentID,
				'pageNo': parameters.pageNo + 1,
				'pageSize': parameters.pageSize,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/childdevicelist', parameter).map(response => {
			return response;
		});
	}


	deleteDevice(deviceGuid) {


		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/hardwarekit/delete/' + deviceGuid, "").map(response => {
			return response;
		});
	}

	uploadPicture(deviceGuid, file) {

		const data = new FormData();
		data.append('image', file);

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/device/' + deviceGuid + '/image', data).map(response => {
			return response;
		});
	}


	getDeviceDetails(deviceGuid) {


		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/' + deviceGuid).map(response => {
			return response;
		});
	}



	addUpdateDevice(data) {

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/device/manage', data).map(response => {
			return response;
		});
	}

	changeStatus(deviceId, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/device/updatestatus/' + deviceId + '/' + status, {}).map(response => {
			return response;
		});
	}
	getallkittypes() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};
        return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/template', configHeader).map(response => {
			
			return response;
		});
		/*return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/alltemplate', configHeader).map(response => {
			
			return response;
		});*/
	}
	getkittypes() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/template', configHeader).map(response => {
			
			return response;
		});
	}
	addUpdateHardwarekit(data,isEdit) {
		
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/hardwarekit/manage?isEdit='+isEdit  , data ).map(response => {
			return response;
		});
	}
	getHardware(parameters) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		const parameter = {
			params: {
				'isAssigned': parameters.isAssigned,
				'pageNo': parameters.pageNo+ 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/hardwarekit/search', reqParameter).map(response => {
			return response;
		});
	}
	getsubscribers(parameters) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		const parameter = {
			params: {
				'pageNo': parameters.pageNo+ 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/search', reqParameter).map(response => {
			return response;
		});
	}
	getHardwarkitDetails(hardwareGuid) {


		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/hardwarekit/' + hardwareGuid).map(response => {
			return response;
		});
	}
	uploadFile(data) {
		
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/hardwarekit/verifykit' , data ).map(response => {
			return response;
		});
	}
	getHardwarkitDownload() {


		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/hardwarekit/download').map(response => {
			return response;
		});
	}
	getsubscriberDetail(params) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		const parameter = {
			params: {
				'userEmail': params.email
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getsubscriberdetails', reqParameter).map(response => {
			return response;
		});
	}

	getSubscriberKitList(parameters) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json'
			}
		};

		const parameter = {
      params: {
        'companyID': parameters.companyID,
				'pageNo': parameters.pageNo+ 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			},
			timestamp: Date.now()
		};
		var reqParameter = Object.assign(parameter, configHeader);

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getsubscriberkitdetails', reqParameter).map(response => {
			return response;
		});
	}

	uploadData(data) {
		
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/hardwarekit/uploadkit' , data ).map(response => {
			return response;
		});
	}

  /**
   * Get List of device details for green house details
   * @param greenhouseGuid
   */
  getDeviceDetailsList(greenhouseGuid) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/device/getgreenhousedevicesdetails/' + greenhouseGuid).map(response => {
      return response;
    });
  }

  getDetailsdevice(deviceGuid) {


	return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/gateway/' + deviceGuid).map(response => {
		return response;
	});
}

getdevicestatics(deviceGuid) {


	return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/dashboard/getdevicedetail/' + deviceGuid).map(response => {
		return response;
	});
}

  /**
   * Get tag lookup
   * @param templateId
   */
  getTagLookup(templateId) {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/attributes/' + templateId).map(response => {

      return response;
    });
  }

  getdevicesensor(deviceGuid,templateId) {


	return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/sensors/' + templateId + '/' + deviceGuid).map(response => {
		return response;
	});
}
}
