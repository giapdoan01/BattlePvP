import re
import os
import requests
url="https://phongtro123.com/tinh-thanh/da-nang/"
html = requests.get(url).text
TieuDe=re.findall('<h3 class="post-title"><a.*?">(.*?)</a></h3>', html)
DonGia=re.findall('<span class="post-price">(.*?)</span>', html)
DienTich=re.findall('<span class="post-acreage">(.*?)</span>', html)
DiaChi=re.findall('<span class="post-location"><a.*?">(.*?)</a></span>', html)
NgayDang=re.findall('<time class="post-time" title=.*?">(.*?)</time>', html)