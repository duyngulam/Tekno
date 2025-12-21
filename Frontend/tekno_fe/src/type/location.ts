export type Province = {
    code: number,
    name: string,
    codename: string,
    divisionType: string,
    phoneCode: number
}
export type District = {
    code: number,
      name: string,
      codename: string,
      divisionType: string,
      provinceCode: number
}
export type Ward = {
    code: number,
      name: string,
      codename: string,
      divisionType: string,
      districtCode: number
}