import { BrowserModule } from '@angular/platform-browser'
import { NgModule } from '@angular/core'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { RxReactiveFormsModule } from '@rxweb/reactive-form-validators'
import { HttpModule } from '@angular/http'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'
import { NgxSpinnerModule } from 'ngx-spinner'
import { CookieService } from 'ngx-cookie-service'
import { SocketIoConfig, SocketIoModule } from 'ng-socket-io'
import { NgxPaginationModule } from 'ngx-pagination'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { TagInputModule } from 'ngx-chips'
import { DateTimeAdapter, OWL_DATE_TIME_FORMATS, OWL_DATE_TIME_LOCALE, OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime'
import { MomentDateTimeAdapter } from 'ng-pick-datetime-moment'
import { MatButtonModule, MatCheckboxModule, MatInputModule, MatProgressBarModule, MatSelectModule, MatSlideToggleModule, MatTabsModule, MatRadioModule } from '@angular/material'
import { Ng2GoogleChartsModule } from 'ng2-google-charts'
import { FullCalendarModule } from '@fullcalendar/angular'
import { AgmCoreModule } from '@agm/core'
import { AgmJsMarkerClustererModule } from '@agm/js-marker-clusterer'
import { AgmDirectionModule } from 'agm-direction'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { PageNotFoundComponent } from './page-not-found.component'
import { MatSidenavModule } from '@angular/material/sidenav'
import { MatTableModule } from '@angular/material/table'
import {
	MatDialogModule, MatIconModule, MatPaginatorModule,
	MatCardModule, MatTooltipModule, MatSortModule
} from '@angular/material'

import { JwtInterceptor } from './helpers/jwt.interceptor';

import { TextMaskModule } from 'angular2-text-mask';
import { AppConstant } from './app.constants';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { ClickOutsideModule } from 'ng-click-outside';

// import custom pipes
import { ShortNumberPipe } from './helpers/pipes/short-number.pipe';
import { ReplacePipe } from './helpers/pipes/replace.pipe';
import { ShortNumberFixnumberPipe } from './helpers/pipes/short-number-fixnumber.pipe';



import {
	GatewayAddDeviceComponent, BulkuploadAddComponent, AdminDashboardComponent, AdminLoginComponent, HomeComponent, RolesListComponent, RolesAddComponent, UserListComponent, UserAddComponent,
	GreenHouseListComponent, GreenHouseAddComponent, RegisterComponent, PaymentComponent,
	PurchasePlanComponent, DeviceAddComponent, FlashMessageComponent, ConfirmDialogComponent, DashboardComponent,
  DeleteDialogComponent, DeviceListComponent, ExtendMeetingComponent, FooterComponent, MessageDialogComponent,
	HeaderComponent, LoginHeaderComponent, LoginFooterComponent, LeftMenuComponent, LoginComponent,
	PageSizeRenderComponent, PaginationRenderComponent, ResetpasswordComponent,
	SearchRenderComponent, SettingsComponent, GatewayAddComponent,
	GatewayListComponent, MyProfileComponent, ChangePasswordComponent,
	CropsListComponent, CropsAddComponent, SubscribersListComponent, HardwareListComponent, HardwareAddComponent, UserAdminListComponent, AdminUserAddComponent, SubscriberDetailComponent,
	NotificationListComponent, NotificationAddComponent, AdminNotificationListComponent, AdminNotificationAddComponent
} from './components/index'

import {
	AuthService, UserService, GreenHouseService, NotificationService,
	DashboardService, DeviceService, RolesService, SettingsService,
	ConfigService, GatewayService, LookupService, SubscriptionService, CropService, RuleService
} from './services/index';
import { GreenHouseDetailsComponent } from './components/green-house/green-house-details/green-house-details.component';
import { TooltipDirective } from './helpers/tooltip.directive';

const config: SocketIoConfig = { url: 'http://localhost:2722', options: {} };
const MY_NATIVE_FORMATS = {
	parseInput: 'DD-MM-YYYY',
	fullPickerInput: 'DD-MM-YYYY hh:mm a',
	datePickerInput: 'DD-MM-YYYY',
	timePickerInput: 'HH:mm',
	monthYearLabel: 'MMM-YYYY',
	dateA11yLabel: 'HH:mm',
	monthYearA11yLabel: 'MMMM-YYYY'
};

@NgModule({
	declarations: [
		AppComponent,
		PageNotFoundComponent,
		LoginComponent,
		HeaderComponent,
		LoginHeaderComponent,
		LoginFooterComponent,
		FooterComponent,
		LeftMenuComponent,
		ExtendMeetingComponent,
		DashboardComponent,
		PageSizeRenderComponent,
		PaginationRenderComponent,
		SearchRenderComponent,
		ConfirmDialogComponent,
    DeleteDialogComponent,
		DeviceListComponent,
		ResetpasswordComponent,
		SettingsComponent,
		FlashMessageComponent,
		DeviceAddComponent,
		UserListComponent,
		UserAddComponent,
		GreenHouseListComponent,
		GreenHouseAddComponent,
		RolesListComponent,
		RolesAddComponent,
		GatewayAddComponent,
		GatewayListComponent,
		MyProfileComponent,
		ChangePasswordComponent,
		CropsListComponent,
		CropsAddComponent,
		HomeComponent,
		RegisterComponent,
		PaymentComponent,
		PurchasePlanComponent,
		ShortNumberPipe,
		ReplacePipe,
		ShortNumberFixnumberPipe,
		AdminLoginComponent,
		AdminDashboardComponent,
		SubscribersListComponent,
		HardwareListComponent,
		HardwareAddComponent,
		UserAdminListComponent,
		AdminUserAddComponent,
		BulkuploadAddComponent,
		SubscriberDetailComponent,
		GreenHouseDetailsComponent,
		GatewayAddDeviceComponent,
		TooltipDirective,
		NotificationListComponent,
		NotificationAddComponent,
		AdminNotificationListComponent,
		AdminNotificationAddComponent,
		MessageDialogComponent
	],
  entryComponents: [DeleteDialogComponent, MessageDialogComponent],
	imports: [
		MatSelectModule,
		MatRadioModule,
		MatButtonModule,
		MatCheckboxModule,
		MatTabsModule,
		MatProgressBarModule,
		MatSlideToggleModule,
		MatInputModule,
		MatSidenavModule,
		MatTableModule,
		MatDialogModule,
		MatIconModule,
		MatPaginatorModule,
		MatSortModule,
		MatCardModule,
		MatTooltipModule,
		BrowserModule,
		TagInputModule,
		BrowserAnimationsModule,
		FormsModule,
		ReactiveFormsModule,
		RxReactiveFormsModule,
		AppRoutingModule,
		HttpModule,
		HttpClientModule,
		NgxSpinnerModule,
		NgxPaginationModule,
		OwlDateTimeModule,
		Ng2GoogleChartsModule,
		OwlNativeDateTimeModule,
		FullCalendarModule,
		SocketIoModule.forRoot(config),
		AgmCoreModule.forRoot({ apiKey: 'AIzaSyDf7yFrQU0RsJpULnEgj8wU6JlGNPeQ6k4' }),
		AgmJsMarkerClustererModule,
		AgmDirectionModule,
		TextMaskModule,
		NgScrollbarModule,
		ClickOutsideModule
	],
	providers: [
		AuthService,
		SettingsService,
		CookieService,
		DeviceService,
		GreenHouseService,
		RolesService,
		DashboardService,
		ConfigService,
		NotificationService,
		GatewayService,
		UserService,
		LookupService,
		RuleService,
		SubscriptionService,
		CropService,
		AppConstant,
		{
			provide: DateTimeAdapter,
			useClass: MomentDateTimeAdapter,
			deps: [OWL_DATE_TIME_LOCALE]
		}, {
			provide: OWL_DATE_TIME_FORMATS,
			useValue: MY_NATIVE_FORMATS
		}, {
			provide: HTTP_INTERCEPTORS,
			useClass: JwtInterceptor,
			multi: true
		}
	],
	bootstrap: [AppComponent]
})

export class AppModule { }
