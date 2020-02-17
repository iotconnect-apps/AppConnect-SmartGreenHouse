import { prop, minDate, maxDate, required, alphaNumeric, alpha, pattern, maxLength, minLength } from '@rxweb/reactive-form-validators';

export class RequestSubscriberFormModel {
  // @required({ message: "Please Select Environment" })
  // @prop()
  // solutionId: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter First Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid First Name" })
  firstName: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter Last Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid Last Name" })
  lastName: string;

  @required({ message: "Please enter Email Address" })
  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @pattern({
    expression: {
      'email': /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$/
    }, message: "Invalid Email"
  })
  email: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: "Please enter Password" })
  @pattern({
    expression: {
      'password': /^(?=.*?[A-Z])(?=.*?[#?!@$%^&*-]).{6,}$/
    }, message: "Invalid Password"
  })
  password: string;

  @required({ message: "Please enter Phone Number" })
  //@pattern({ expression: { 'companyPhone': /^[\+\d]?(?:[\d-.\s()]*)$/i } })
  @pattern({
    expression: {
      'phoneNumber': /^[0-9]*$/i
    }, message: "Please enter valid Phone Number."
  })
  @minLength({ value: 8,message:'please enter minimum 8 digit' })
  @maxLength({ value: 10,message:'please enter maximum 10 digit' })
  phoneNumber: string = "";

  @required({ message: "Please enter Country Code." })
  //@pattern({ expression: { 'companyPhone': /^[\+\d]?(?:[\d-.\s()]*)$/i } })
  @pattern({
    expression: {
      'ccode': /^[0-9]*$/i
    }, message: "Please enter Country Code."
  })
  @maxLength({ value: 3,message:'please enter maximum 3 digit' })
  ccode: string = "";




  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter City Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid City name" })
  cityName: string;
  // @prop()
  // @required({message: "Enter Postal Code" })
  // @numeric({ acceptValue:NumericValueType.PositiveNumber  ,allowDecimal:false ,message: "Invalid Postal Code"}) 
  // postalCode: string;

  @prop()
  @maxLength({ value: 10, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: "Please enter Postal Code" })
  @pattern({
    expression: {
      'companyPostalCode': /^[a-z\d\-\s]+$/i
    }, message: "Please enter valid Postal Code"
  })
  // @alphaNumeric({ allowWhiteSpace: true, message: "Please enter valid Postal code" })
  postalCode: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter Company Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid Company Name" })
  companyName: string;

  @required({ message: "Please enter Address" })
  // @alphaNumeric({ allowWhiteSpace: true, message: "" })
  //@alpha({ allowWhiteSpace: true, message: "Invalid Address" })
  address: string;
  @required({ message: "Please Select Time Zone" })
  timezoneId: string;
  @prop()
  isSameSolutionInstance: boolean = false;

  // @required({ message: "Please Select Product" })
  // @prop()
  // productCode: string;
  @required({ message: "Please Select Country" })
  countryName: string;
  @required({ message: "Please Select State" })
  stateName: string;
}
