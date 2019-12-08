(window.webpackJsonp=window.webpackJsonp||[]).push([[1],{11:function(e,n,a){"use strict";a.d(n,"b",(function(){return l})),a.d(n,"a",(function(){return p}));const t="https://raw.githubusercontent.com/",r="L3tum",o="CPU-Benchmark-Database",c="master",s="saves",i="aggregations";function l(e){return`${t}${r}/${o}/${c}/${s}/${e}.json`}function p(e){return`${t}${r}/${o}/${c}/${i}/pagination/${e}.json`}},9:function(e,n,a){"use strict";a.r(n),a.d(n,"getReferencePoints",(function(){return h})),a.d(n,"renderResults",(function(){return d})),a.d(n,"renderInfo",(function(){return f})),a.d(n,"render",(function(){return m}));var t=a(0),r=a.n(t),o=a(11);const c='\n<div class="col-12 d-flex justify-content-end"><h5>[save]</h5></div>\n<div class="col-12 d-flex justify-content-between"><h2>[cpu]</h2><h2>[score] Points <span class="score [score-class]" title="Faster (green) or slower (orange) than the reference 3900X"></span></h2></div>\n<div class="col-12 col-lg-6" style="margin-top: 50px">\n<h5>Results</h5>\n<table class="table">\n<thead>\n<tr>\n<th>Benchmark</th>\n<th>Score</th>\n</tr>\n</thead>\n<tbody>\n[results]\n</tbody>\n</table>\n</div>\n<div class="col-12 col-lg-6" style="margin-top: 50px">\n<h5>Machine</h5>\n<table class="table">\n<thead>\n<tr>\n<th>Property</th>\n<th>Value</th>\n</tr>\n</thead>\n<tbody>\n[info]\n</tbody>\n</table>\n</div>\n',s="\n<tr>\n    <td>[name]</td>\n    <td>[value]</td>\n</tr>\n",i='\n<tr [optional-class]>\n    <td>[name]</td>\n    <td>[value] <span class="score [score-class]" title="Faster (green) or slower (orange) than the [reference] of [reference-points]"></span></td>\n</tr>\n';function l(){r()("#accordion").html("<h5>Can't find the specified save!</h5>")}let p=!1,u=null;function h(e,n){let a=5e4;if(!p)return a;if(console.log(u),!Object.keys(u.Results).includes(e))return a;if(!Array.isArray(u.Results[e]))return a;return u.Results[e].forEach((function(e){e.Benchmark===n&&(a=e.Points)})),a}function d(e){let n=[];return Object.keys(e.Results).forEach((function(a){Array.isArray(e.Results[a])&&e.Results[a].forEach((function(e){const t=h(a,e.Benchmark);let r=i.replace("[value]",e.Points).replace("[name]",`${e.Benchmark} @ ${a} ${parseInt(a),"Threads"}`).replace("[score-class]",e.Points>=t?"above":"below").replace("[reference]",p?"average":"reference 3900x").replace("[reference-points]",t);e.Benchmark.startsWith("Category: all")?r=r.replace("[optional-class]",'class="category-all"'):e.Benchmark.startsWith("Category:")&&(r=r.replace("[optional-class]",'class="category"')),n.push(r)}))})),n.sort((function(e,n){const a=e.includes("Category"),t=e.includes("Category: all"),r=n.includes("Category"),o=n.includes("Category: all");return t?-1:o?1:a&&!r?-1:!a&&r?1:0})),n.join("")}function f(e){let n="";n+=s.replace("[name]","Caption").replace("[value]",e.MachineInformation.Cpu.Caption),n+=s.replace("[name]","Vendor").replace("[value]",e.MachineInformation.Cpu.Vendor),n+=s.replace("[name]","Cores").replace("[value]",e.MachineInformation.Cpu.PhysicalCores),n+=s.replace("[name]","Threads").replace("[value]",e.MachineInformation.Cpu.LogicalCores),n+=s.replace("[name]","NUMA").replace("[value]",`${e.MachineInformation.Cpu.Nodes} Node${1===e.MachineInformation.Cpu.Nodes?"":"s"} @ ${e.MachineInformation.Cpu.LogicalCoresPerNode} Threads per Node`),n+=s.replace("[name]","Frequency").replace("[value]",`${(e.MachineInformation.Cpu.MaxClockSpeed/1e3).toFixed(2)} GHz Measured / ${(e.MachineInformation.Cpu.NormalClockSpeed/1e3).toFixed(2)} GHz Reported`),n+=s.replace("[name]","Socket").replace("[value]",e.MachineInformation.Cpu.Socket),n+=s.replace("[name]","BIOS").replace("[value]",`${e.MachineInformation.SmBios.BIOSCodename} ${e.MachineInformation.SmBios.BIOSVersion} by ${e.MachineInformation.SmBios.BIOSVendor}`),n+=s.replace("[name]","Mainboard").replace("[value]",`${e.MachineInformation.SmBios.BoardName} ${e.MachineInformation.SmBios.BoardVersion} by ${e.MachineInformation.SmBios.BoardVendor}`);let a="";e.MachineInformation.Cpu.Cores.forEach((function(n){a+=`#${n.Number.toString().padStart(2,"0")} ${(e.MachineInformation.Cpu.MaxClockSpeed/1e3).toFixed(2)} GHz${(n.Number+1)%3==0?"\n":"\t"}`})),n+=s.replace("[name]","Cores").replace("[value]",`<span style="white-space: pre">${a}</span>`);let t="";e.MachineInformation.Cpu.Caches.forEach((function(e){t+=`L${e.Level}\t${e.CapacityHRF}\t${e.Associativity}-way\t${e.TimesPresent}-times\t${1===e.Type?"Instruction":2===e.Type?"Data":""}\n`})),n+=s.replace("[name]","Caches").replace("[value]",`<span style="white-space: pre">${t}</span>`);let r="";return e.MachineInformation.RAMSticks.forEach((function(e,n){r+=`${e.Name?e.Name:n} ${e.CapacityHRF} @ ${e.Speed} Mhz by ${e.Manfucturer}\n`})),n+=s.replace("[name]","RAM").replace("[value]",`<span style="white-space: pre">${r}</span>`),n}function m(){r()("#sorting").parent().hide(),r()("#sorting").hide(),r()("#prev").parent().hide();const e=window.location.search.replace("?detail=","");fetch(Object(o.b)(e)).then(n=>{n.ok&&404!==n.status?n.json().then(a=>{fetch(Object(o.b)(`average_${a.MachineInformation.Cpu.Caption.replace(/@/g,"at").replace(/ /g,"_").replace(/,/g,"_")}.automated`)).then(e=>new Promise((a,t)=>{n.ok&&404!==n.status?(p=!0,e.json().then(e=>{u=e,a()}).catch(()=>{a()})):a()})).then(()=>{let n=c.replace(/\[save]/g,e).replace(/\[cpu]/g,a.MachineInformation.Cpu.Name);n=n.replace("[info]",f(a)),n=n.replace("[results]",d(a)),n=n.replace(/\[optional-class]/g,"");let t=Object.keys(a.Results),o=0,s=0;t.forEach((function(e){if(!Array.isArray(a.Results[e]))return;let n=0,t=0;a.Results[e].forEach((function(e){e.Benchmark.startsWith("Category:")||(n+=e.Points,t++)})),o+=n,s+=t}));const i=o/s;n=n.replace(/\[score]/g,isNaN(i)?"0".padStart(5,"0"):i.toFixed(0).padStart(5,"0")),n=n.replace(/\[score-class]/g,isNaN(i)?"below":i<5e4?"below":5e4===i?"same":"above"),r()("#accordion").html(n),r()(".navbar").addClass(a.MachineInformation.Cpu.Vendor)})}).catch(e=>{console.error(e),l()}):l()})}}}]);