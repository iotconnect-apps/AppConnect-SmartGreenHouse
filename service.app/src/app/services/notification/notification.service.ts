import { Injectable } from '@angular/core'
import { Subject } from 'rxjs/Subject'

export class Notification {
	constructor(
		public type: string = '',
		public message: string = ''
	) { }
}

@Injectable({
	providedIn: 'root'
})

export class NotificationService {
	refreshTokenInProgress = false;
	apiBaseUrl = '';
	constructor() { }

	private _notifications = new Subject<Notification>();
	public noteAdded = this._notifications.asObservable();

	public add(notification: Notification) {
		if (notification.message !== "Unauthorized") {
			this._notifications.next(notification);
		}
	}
}

export class ResponseData {
	success: boolean;
	message: any;
	isflash: boolean;
	imageUrlPath: string;
	img: string;
	image: string;
	id: string;
	cms_id: string;
}