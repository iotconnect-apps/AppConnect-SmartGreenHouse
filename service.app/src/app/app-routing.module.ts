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
	AlertsComponent, BulkuploadAddComponent, NotificationListComponent, NotificationAddComponent, AdminNotificationListComponent, AdminNotificationAddComponent
} from './components/index'

import { AuthService, AdminAuthGuired } from './services/index'

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
				path: 'subscribers/:email/:productCode/:companyId',
				component: SubscriberDetailComponent,
				canActivate: [AuthService]
			},
			{
				path: 'subscribers',
				component: SubscribersListComponent,
				canActivate: [AuthService]
			},
			{
				path: 'hardwarekits',
				component: HardwareListComponent,
				canActivate: [AuthService]
      },
      {
        path: 'hardwarekits/bulkupload',
        component: BulkuploadAddComponent,
        canActivate: [AuthService]
      },
			{
				path: 'hardwarekits/add',
				component: HardwareAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'hardwarekits/:hardwarekitGuid',
				component: HardwareAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'users',
				component: UserAdminListComponent,
				canActivate: [AuthService]
			},
			{
				path: 'users/add',
				component: AdminUserAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'users/:userGuid',
				component: AdminUserAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notifications/:notificationGuid',
				component: AdminNotificationAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notifications/add',
				component: AdminNotificationAddComponent,
				canActivate: [AuthService]
			},
			{
				path: 'notifications',
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
		// canActivate: [AdminAuthGuired]
	},
	{
		path: 'change-password',
		component: ChangePasswordComponent,
		// canActivate: [AdminAuthGuired]
	},
	{
		path: 'dashboard',
		component: DashboardComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'green-house/:greenHouseGuid',
		component: GreenHouseAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'green-house/add',
		component: GreenHouseAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'green-house-details/:greenHouseGuid',
		component: GreenHouseDetailsComponent,
		pathMatch: 'full',
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'green-houses',
		component: GreenHouseListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'users/:userGuid',
		component: UserAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'users/add',
		component: UserAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'users',
		component: UserListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'device/:parentDeviceGuid',
		component: DeviceAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'device/:parentDeviceGuid/:childDeviceGuid',
		component: DeviceAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'devices',
		component: DeviceListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'roles/:deviceGuid',
		component: RolesAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'roles',
		component: RolesListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'gateways/:gatewayGuid',
		component: GatewayAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'gateways/add',
		component: GatewayAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'gateways',
		component: GatewayListComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'crops/add',
		component: CropsAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'crops/:cropsGuid',
		component: CropsAddComponent,
		canActivate: [AdminAuthGuired]
	}, {
		path: 'crops',
		component: CropsListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'notification/:notificationGuid',
		component: NotificationAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'notification/add',
		component: NotificationAddComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'notifications',
		component: NotificationListComponent,
		canActivate: [AdminAuthGuired]
	},
	{
		path: 'alerts',
		component: AlertsComponent,
		canActivate: [AdminAuthGuired]
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
