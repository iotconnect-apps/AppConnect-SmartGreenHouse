import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GatewayAddDeviceComponent } from './gateway-add-device.component';

describe('GatewayAddDeviceComponent', () => {
  let component: GatewayAddDeviceComponent;
  let fixture: ComponentFixture<GatewayAddDeviceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GatewayAddDeviceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GatewayAddDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
