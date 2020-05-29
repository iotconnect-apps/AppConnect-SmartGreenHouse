import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiConfigService,NotificationService } from 'app/services';
@Injectable({
  providedIn: 'root'
})
export class CropService {
  protected apiServer = ApiConfigService.settings.apiServer;
  constructor(private httpClient: HttpClient,
    private _notificationService: NotificationService) {
			this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
		}

  /**
   * Get crop list
   * @param greenHouseId
   */
  getCrop(greenHouseId) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/crop/crops/' + greenHouseId).map(response => {
      return response;
    });
  }

  removeCropImage(entityId) {
		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/crop/deleteimage/'+entityId,{}).map(response => {
		  return response;
		});
	  }

/**
 * Ger crop details
 * @param cropId
 */
  getCropDetails(cropId) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/crop/' + cropId).map(response => {
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

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/crop/manage', formData).map(response => {
      return response;
    });
  }

  /**
   * Delete Crop
   * @param cropGuid
   */
  deleteCrop(cropGuid) {

    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/crop/delete/' + cropGuid, {}).map(response => {
      return response;
    });
  }
}
