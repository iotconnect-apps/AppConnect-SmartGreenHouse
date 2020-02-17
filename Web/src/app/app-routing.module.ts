import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { SelectivePreloadingStrategy } from './selective-preloading-strategy'
import { PageNotFoundComponent } from './page-not-found.component'
import {
	HomeComponent, UserListComponent, UserAddComponent, GreenHouseListComponent, GreenHouseDetailsComponent,
	GreenHouseAddComponent, DeviceAddComponent, DashboardComponent, DeviceListComponent,
	LoginComponent, RegisterComponent, MyProfileComponent, ResetpasswordComponent, SettingsComponent,
	RolesListComponent, RolesAddComponent, GatewayAddComponent, GatewayListComponent,
	ChangePasswordComponent, CropsAddComponent, CropsListComponent, AdminLoginComponent, SubscribersListComponent, HardwareListComponent, HardwareAddComponent, UserAdminListComponent, AdminUserAddComponent, AdminDashboardComponent, SubscriberDetailComponent,
	BulkuploadAddComponent, NotificationListComponent, NotificationAddComponent, AdminNotificationListComponent, AdminNotificationAddComponent
} from './components/index'

import { AuthService } from './services/index'

const appRoutes: Routes = [
	{
		path: 'admin',
		children: [
			{
				path: '',
				component: AdminLoginComponent
			},
			{
				path: 'dashboard',
				component: AdminDashboardComponent,
				canActivate: [AuthService]
			},
			{
				path: 'subscriber/:email/:companyId',
				component: SubscriberDetailComponent,
				canActivate: [AuthService]
			},
			{
				path: 'subscriber',
				component: SubscribersListComponent,
				canActivate: [AuthService]
			},
			{
				path: 'viewhardwarekit',
				component: HardwareListComponent,
				canActivate: [AuthService]
			},
			{
				path: 'hardwarekit/addhardwarekit',
				component: HardwareAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'hardwarekit/:hardwarekitGuid',
				component: HardwareAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'viewusers',
				component: UserAdminListComponent,
				canActivate: [AuthService]
			},
			{
				path: 'user/adduser',
				component: AdminUserAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'user/:userGuid',
				component: AdminUserAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'bulkupload',
				component: BulkuploadAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notification/:notificationGuid',
				component: AdminNotificationAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notification/add',
				component: AdminNotificationAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notification',
				component: AdminNotificationListComponent,
				canActivate: [AuthService]
			}
		]
	},
	{
		path: '',
		component: HomeComponent
	},
	{
		path: 'login',
		component: LoginComponent
	},
	{
		path: 'register',
		component: RegisterComponent
	},
	//App routes goes here 
	{
		path: 'my-profile',
		component: MyProfileComponent,
		canActivate: [AuthService]
	},
	{
		path: 'change-password',
		component: ChangePasswordComponent,
		canActivate: [AuthService]
	},
	{
		path: 'dashboard',
		component: DashboardComponent,
		canActivate: [AuthService]
	},
	{
		path: 'green-house/:greenHouseGuid',
		component: GreenHouseAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'green-house/add',
		component: GreenHouseAddComponent,
		//canActivate: [AuthService]
	},
	{
		path: 'green-house-details/:greenHouseGuid',
		component: GreenHouseDetailsComponent,
		pathMatch: 'full'
		//canActivate: [AuthService]
	},
	{
		path: 'green-houses',
		component: GreenHouseListComponent,
		canActivate: [AuthService]
	}, {
		path: 'user/:userGuid',
		component: UserAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'user/add',
		component: UserAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'users',
		component: UserListComponent,
		canActivate: [AuthService]
	}, {
		path: 'device/:parentDeviceGuid',
		component: DeviceAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'device/:parentDeviceGuid/:childDeviceGuid',
		component: DeviceAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'devices',
		component: DeviceListComponent,
		canActivate: [AuthService]
	}, {
		path: 'roles/:deviceGuid',
		component: RolesAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'roles',
		component: RolesListComponent,
		canActivate: [AuthService]
	}, {
		path: 'gateways/:gatewayGuid',
		component: GatewayAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'gateways/add',
		component: GatewayAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'gateways',
		component: GatewayListComponent,
		canActivate: [AuthService]
	}, {
		path: 'crops/add',
		component: CropsAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'crops/:cropsGuid',
		component: CropsAddComponent,
		canActivate: [AuthService]
	}, {
		path: 'crops',
		component: CropsListComponent,
		canActivate: [AuthService]
	},

	{
		path: 'notification/:notificationGuid',
		component: NotificationAddComponent,
		canActivate: [AuthService]
	},
	{
		path: 'notification/add',
		component: NotificationAddComponent,
		canActivate: [AuthService]
	},
	{
		path: 'notification',
		component: NotificationListComponent,
		//canActivate: [AuthService]
	},

	{
		path: '**',
		component: PageNotFoundComponent
	}
];

@NgModule({
	imports: [
		RouterModule.forRoot(
			appRoutes, {
			preloadingStrategy: SelectivePreloadingStrategy
		}
		)
	],
	exports: [
		RouterModule
	],
	providers: [
		SelectivePreloadingStrategy
	]
})

export class AppRoutingModule { }
