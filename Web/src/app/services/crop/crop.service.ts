import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from './../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CropService {

  constructor(private httpClient: HttpClient) { }

  /**
   * Get crop list
   * @param greenHouseId
   */
  getCrop(greenHouseId) {

    return this.httpClient.get<any>(environment.baseUrl + 'crop/crops/' + greenHouseId).map(response => {
      return response;
    });
  }


/**
 * Ger crop details
 * @param cropId
 */
  getCropDetails(cropId) {

    return this.httpClient.get<any>(environment.baseUrl + 'crop/' + cropId).map(response => {
      return response;
    });
  }

  /**
   * Add/Update Crop
   * @param data
   */
  manageCrop(data) {

    const formData = new FormData();
    for (const key of Object.keys(data)) {
      const value = data[key];
      formData.append(key, value);
    }

    return this.httpClient.post<any>(environment.baseUrl + 'crop/manage', formData).map(response => {
      return response;
    });
  }

  /**
   * Delete Crop
   * @param cropGuid
   */
  deleteCrop(cropGuid) {

    return this.httpClient.put<any>(environment.baseUrl + 'crop/delete/' + cropGuid, {}).map(response => {
      return response;
    });
  }
}
