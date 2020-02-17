import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminNotificationAddComponent } from './admin-notification-add.component';

describe('AdminNotificationAddComponent', () => {
  let component: AdminNotificationAddComponent;
  let fixture: ComponentFixture<AdminNotificationAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdminNotificationAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminNotificationAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
