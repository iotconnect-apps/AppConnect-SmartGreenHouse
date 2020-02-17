import { Component, OnInit } from '@angular/core'
import { Router } from '@angular/router'


@Component({
	selector: 'app-left-menu',
	templateUrl: './left-menu.component.html',
	styleUrls: ['./left-menu.component.css']
})

export class LeftMenuComponent implements OnInit {
	menuList: any = [];
	user_type: any;
	selectedMenu: any;
	currentUrl: any;
	isAdmin = false;
	constructor(private router: Router) { }

	ngOnInit() {
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		this.isAdmin = currentUser.userDetail.isAdmin;
		let tempUrl = (this.router.url).split('/');
		this.currentUrl = "/" + tempUrl[1];
		if (this.isAdmin) {
			this.menuList = [
				{
					name: 'Dashboard',
					link: '/admin/dashboard',
					li_color: '',
					icon: 'icon-dashboard',
					applylink: true
				},
				{
					name: 'Hardware Kit',
					link: '/admin/viewhardwarekit',
					li_color: '',
					icon: 'icon-devices',
					applylink: true
				},
				{
					name: 'User Management',
					link: '/admin/viewusers',
					li_color: '',
					icon: 'icon-user-group',
					applylink: true
				},
				{
					name: 'Subscribers',
					link: '/admin/subscriber',
					li_color: '',
					icon: 'icon-subscribers',
					applylink: true
				},
				{
					name: 'Notification',
					link: '/admin/notification',
					li_color: '',
					icon: 'icon-notification',
					applylink: true
				},
				
			];
		} else {
			this.menuList = [
				{
					name: 'Dashboard',
					link: '/dashboard',
					li_color: '',
					icon: 'icon-dashboard',
					applylink: true
				},
				{
					name: "Green-Houses",
					link: "/green-houses",
					li_color: '',
					icon: 'icon-greenhouse',
					applylink: true
				},
				{
					name: "Roles",
					link: "/roles",
					li_color: '',
					icon: 'icon-userrole',
					applylink: true
				},
				{
					name: "User Management",
					link: "/users",
					li_color: '',
					icon: 'icon-user-group',
					applylink: true
				},
				{
          name: "Hardware Kit",
					link: "/gateways",
					li_color: '',
					icon: 'icon-devices',
					applylink: true
				},
				{
					name: 'Notification',
					link: '/notification',
					li_color: '',
					icon: 'icon-notification',
					applylink: true
				},	
				// {	
				// 	name: "Devices",
				// 	link: "/devices",
				// 	li_color: '',
				// 	icon: 'icon-devices',
				// 	applylink: true
				// },
				// {	
				// 	name: "Crops",
				// 	link: "/crops",
				// 	li_color: '',
				// 	icon: 'icon-simanagement',
				// 	applylink: true
				// },
			];
		}

	}

	manageNavigateUrl(url) {
		this.router.navigate([url]);
	}

	showSubMenu(menu) {
		menu.showSunMenu = !menu.showSunMenu;
	}

	onClickMenu(i) {
		this.currentUrl = false;
		this.selectedMenu = i;
	}
}
