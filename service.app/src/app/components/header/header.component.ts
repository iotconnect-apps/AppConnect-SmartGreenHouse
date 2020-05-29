import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  ChangeDetectorRef
} from "@angular/core";
import { TitleCasePipe } from "@angular/common";
import { Router, NavigationEnd } from "@angular/router";

import { AuthService, RuleService } from "../../services/index";
import { StompRService } from "@stomp/ng2-stompjs";
import { Subscription } from "rxjs";
import { Observable } from "rxjs/Observable";
import { Message } from "@stomp/stompjs";
import * as moment from "moment";
import { AppConstant } from "../../app.constants";
// import { ConfigService } from './../../services/index'

@Component({
  selector: "app-header",
  templateUrl: "./header.component.html",
  styleUrls: ["./header.css"],
  providers: [TitleCasePipe, StompRService]
})
export class HeaderComponent implements OnInit, OnDestroy {
  isAdmin = false;
  cookieName = "FM";
  userName;
  isMenuOpen: boolean = false;
  cpId = "";
  stompConfiguration = {
    url: "",
    headers: {
      login: "",
      passcode: "",
      host: ""
    },
    heartbeat_in: 0,
    heartbeat_out: 2000,
    reconnect_delay: 5000,
    debug: true
  };
  subscription: Subscription;
  messages: Observable<Message>;
  subscribed;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  alerts = [];
  unReadAlerts = 0;
  respondShow: boolean = false;
  @ViewChild("showDropdown", { static: false }) public elementRef: ElementRef;
  constructor(
    public router: Router,
    private cd: ChangeDetectorRef,
    private authService: AuthService,
    private stompService: StompRService,
    private ruleService: RuleService,
    public _appConstant : AppConstant
  ) {
    router.events.subscribe(val => {
      if (val instanceof NavigationEnd) {
        if (this.elementRef)
          this.elementRef.nativeElement.classList.remove("show");
      }
    });
  }

  ngOnInit() {
    let currentUser = JSON.parse(localStorage.getItem("currentUser"));
    this.authService.updateUserNameData.subscribe(data => {
      if (data) {
        this.userName = data;
      } else {
        this.userName = currentUser.userDetail.fullName;
      }
      this._appConstant.username = this.userName;
    });

    this.isAdmin = currentUser.userDetail.isAdmin;

    this.getStompConfig();
  }
  ngOnDestroy() {
    this.unsubscribe();
    this.stompService.disconnect();
  }

  public unsubscribe() {
    if (!this.subscribed) {
      return;
    }
    // This will internally unsubscribe from Stomp Broker
    // There are two subscriptions - one created explicitly, the other created in the template by use of 'async'
    this.subscription.unsubscribe();
    this.subscription = null;
    this.messages = null;

    this.subscribed = false;
  }

  logout() {
    this.authService.logout();
		if(this.currentUser.userDetail.isAdmin){
			this.router.navigate(['/admin'])
		} else {
			this.router.navigate(['/login'])
		}
  }

  onClickedOutside(e) {
    if (
      e.path[0].className == "dropdown-toggle" ||
      e.path[1].className == "dropdown-toggle" ||
      e.path[2].className == "dropdown-toggle"
    ) {
      return false;
    }
    this.isMenuOpen = false;
  }

  getStompConfig() {
    if (this.currentUser.userDetail.cpId) {
      this.ruleService.getStompConfig("UIAlert").subscribe(response => {
        if (response.isSuccess) {
          if(response.data.url){
            let cpId = this.currentUser.userDetail.cpId;
            this.stompConfiguration.url = response.data.url;
            this.stompConfiguration.headers.login = response.data.user;
            this.stompConfiguration.headers.passcode = response.data.password;
            this.stompConfiguration.headers.host = response.data.vhost;
            this.cpId = cpId.toLowerCase();
            if(cpId){
              this.initStomp();
            }
          }
        }
      });
    }
  }

  initStomp() {
    let config = this.stompConfiguration;
    this.stompService.config = config;
    this.stompService.initAndConnect();
    this.stompSubscribe();
  }

  public stompSubscribe() {
    if (this.subscribed) {
      return;
    }

    this.messages = this.stompService.subscribe("/topic/" + this.cpId);
    this.subscription = this.messages.subscribe(this.on_next);
    this.subscribed = true;
  }

  public on_next = (message: Message) => {
    let user_id = this.currentUser.userDetail.id;
    let obj: any = JSON.parse(message.body);
    let users = obj.data.data.users;
    if (users.length && user_id) {
      let isAlert = users.find(o => o.guid === user_id);
      if (isAlert) {
        let now = moment(obj.data.data.time).format("MMM DD, YYYY h:mm:ss A");
        this.alerts.unshift({
          time: now,
          message: obj.data.data.data.message,
          severity: obj.data.data.severity.toLowerCase()
        });
        this.unReadAlerts++;
      }
    }
  };
  public stompUnsubscribe() {
    this.stompService.disconnect();
    this.subscribed = false;
  }
  Respond() {
    this.respondShow = !this.respondShow;
    if (!this.respondShow) {
      this.alerts = [];
      this.unReadAlerts = 0;
    }
  }
}
