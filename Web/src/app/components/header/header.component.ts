import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core'
import { TitleCasePipe } from '@angular/common'
import { Router, NavigationEnd } from '@angular/router';

import { UserService, AuthService } from '../../services/index';
// import { ConfigService } from './../../services/index'

@Component({
	selector: 'app-header',
	templateUrl: './header.component.html',
	styleUrls: ['./header.css'],
	providers: [TitleCasePipe]
})

export class HeaderComponent implements OnInit {
	cookieName = 'FM';
	userName;
	isMenuOpen: boolean = false;

	@ViewChild('showDropdown', { static: false }) private elementRef: ElementRef;

	constructor(
		public router: Router,
		private cd: ChangeDetectorRef,
		private authService: AuthService,


		// private configService:ConfigService
	) {

		router.events.subscribe((val) => {
			if (val instanceof NavigationEnd) {
				this.elementRef.nativeElement.classList.remove("show");
			}
		});

	}

	ngOnInit() {
		this.userName = 'Admin';
	}

	logout() {
		this.authService.logout();
		this.router.navigate(['/']);
	}

	onClickedOutside(e) {
		if (e.path[0].className == "dropdown-toggle" || e.path[1].className == "dropdown-toggle" || e.path[2].className == "dropdown-toggle") {
			return false;
		}
		this.isMenuOpen = false;
	}
}