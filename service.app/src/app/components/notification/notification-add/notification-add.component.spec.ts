import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationAddComponent } from './notification-add.component';

describe('NotificationAddComponent', () => {
  let component: NotificationAddComponent;
  let fixture: ComponentFixture<NotificationAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotificationAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
