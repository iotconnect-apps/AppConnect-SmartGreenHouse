import { Injectable } from '@angular/core'
import { CanActivate, Router } from '@angular/router'

@Injectable({
	providedIn: 'root'
})

export class AuthService implements CanActivate {
	constructor(private router: Router) { }

	canActivate() {
		if (this.isCheckLogin()) {
			return true;
		} else {
			this.removeAllStorage();
			this.router.navigate(['/login']);
			return false;
		}
	}

	isCheckLogin() {
		// check if the user is logged in
		if (localStorage.getItem('currentUser')) {
			return true;
		}
		return false;

	}

	logout(){
		this.removeAllStorage();
	}

	removeAllStorage() {
		localStorage.clear();
	}
}