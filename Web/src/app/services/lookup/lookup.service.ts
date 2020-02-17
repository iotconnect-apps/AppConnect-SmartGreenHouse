import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { environment } from '@environment/environment';

import 'rxjs/add/operator/map'


@Injectable({
  providedIn: 'root'
})
export class LookupService {

  constructor(
    private httpClient: HttpClient

  ) { }


  // tag look up based on template ID or code
  getTagsLookup(tagID) {

    return this.httpClient.get<any>(environment.baseUrl + 'lookup/taglookup/' + tagID).map(response => {
      return response;
    });

  }

  // get time zone list
  getTimezoneList() {
    return this.httpClient.get<any>(environment.baseUrl + 'Lookup/timezone').map(response => {
      return response;
    });
  }

  // get country list
  getcountryList() {

    return this.httpClient.get<any>(environment.baseUrl + 'lookup/country').map(response => {
      return response;
    });
  }

  // get city list based on country ID
  getcitylist(countryId) {

    return this.httpClient.get<any>(environment.baseUrl + 'lookup/state/' + countryId).map(response => {
      return response;
    });
  }


  // NoAuth APIs 

  // get time zone list
  getNoAuthTimezoneList() {
    return this.httpClient.get<any>(environment.baseUrl + 'subscriber/gettimezonelookup').map(response => {
      return response;
    });
  }

  // get country list
  getNoAuthcountryList() {
    return this.httpClient.get<any>(environment.baseUrl + 'subscriber/getcountrylookup').map(response => {
      return response;
    });
  }

  // get city list based on country ID
  getNoAuthcitylist(countryId) {
    return this.httpClient.get<any>(environment.baseUrl + 'subscriber/getstatelookup/' + countryId).map(response => {
      return response;
    });
  }

  // get subscription plans list
  getNoAuthplanslist() {
    return this.httpClient.get<any>(environment.baseUrl + 'subscriber/getsubscriptionplan').map(response => {
      return response;
    });
  }

  postNoAuthRegister(data) {
    return this.httpClient.post<any>(environment.baseUrl + 'subscriber/company', data).map(response => {
      return response;
    });

  }

  // get subscription plans list
  getNoAuthSripeKey() {
    return this.httpClient.get<any>(environment.baseUrl + 'subscriber/getstripekey').map(response => {
      return response;
    });
  }

}
