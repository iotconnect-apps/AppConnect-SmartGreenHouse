import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { ApiConfigService, NotificationService } from 'app/services';

import 'rxjs/add/operator/map'


@Injectable({
  providedIn: 'root'
})
export class LookupService {
  protected apiServer = ApiConfigService.settings.apiServer;

  constructor(
    private httpClient: HttpClient,
    private _notificationService: NotificationService) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }


  // tag look up based on template ID or code
  getTagsLookup(tagID) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/taglookup/' + tagID).map(response => {
      return response;
    });

  }

  // get time zone list
  getTimezoneList() {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/Lookup/timezone').map(response => {
      return response;
    });
  }

  // get country list
  getcountryList() {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/country').map(response => {
      return response;
    });
  }

  // get state list based on country ID
  getstateList(countryId) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/lookup/state/' + countryId).map(response => {
      return response;
    });
  }


  // NoAuth APIs 

  // get time zone list
  getNoAuthTimezoneList() {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/gettimezonelookup').map(response => {
      return response;
    });
  }

  // get country list
  getNoAuthcountryList() {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getcountrylookup').map(response => {
      return response;
    });
  }

  // get city list based on country ID
  getNoAuthcitylist(countryId) {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getstatelookup/' + countryId).map(response => {
      return response;
    });
  }

  // get subscription plans list
  getNoAuthplanslist() {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getsubscriptionplan').map(response => {
      return response;
    });
  }

  postNoAuthRegister(data) {
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/subscriber/company', data).map(response => {
      return response;
    });

  }

  // get subscription plans list
  getNoAuthSripeKey() {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/subscriber/getstripekey').map(response => {
      return response;
    });
  }

}
