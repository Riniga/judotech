/// <binding BeforeBuild='default' />
import gulp from 'gulp';
import clean from 'gulp-clean';
import pug from 'gulp-pug';

import yargs from 'yargs'
import { hideBin } from 'yargs/helpers'
const argv = yargs(hideBin(process.argv))
  .option('environment', {
    alias: 'e',
    type: 'string',
    description: 'What environment to build for [production || test]',
    default: 'development',
    demandOption: false
  }).argv;

  console.log(`Environment: ${argv.environment}`);


gulp.task('clean', function () {
    return gulp.src('./public', { read: false, allowEmpty: true })
        .pipe(clean());
});

gulp.task('pug', function () {
    return gulp.src('./source/pug/pages/**/*.pug')
        .pipe(pug({ pretty: true }))
        .pipe(gulp.dest('./public'));
});

gulp.task('scripts_old', function () {
    return gulp.src('./source/scripts/**/*.js')
        .pipe(gulp.dest('./public/scripts'));
});

import fs from 'node:fs';
import concat from 'gulp-concat'
import uglify from 'gulp-uglify'
gulp.task('scripts', function () 
{
  const LAYOUT_SOURCE_DIR = "../judotech.web/scripts/"; 
  const PROJECT_SOURCE_DIR = "source/scripts/";
  const OUTPUT_DIR = "public/scripts/";
  
  const layout_folders = fs.readdirSync(LAYOUT_SOURCE_DIR, { withFileTypes: true })
    .filter(dirent => dirent.isDirectory()) 
    .map(dirent => dirent.name);

  const project_folders = fs.readdirSync(PROJECT_SOURCE_DIR, { withFileTypes: true })
    .filter(dirent => dirent.isDirectory() && dirent.name != "config") 
    .map(dirent => dirent.name);

    const tasks = layout_folders.map(folder => {
        var stream = gulp.src(`${LAYOUT_SOURCE_DIR}${folder}/*.js`) 
            .pipe(concat(`${folder}.js`)) 
            //.pipe(rename({ extname: ".min.js" })) 
          if (argv.environment=='production') stream = stream.pipe(uglify())
        return stream.pipe(gulp.dest(OUTPUT_DIR));
    });

    const project_tasks = project_folders.map(folder => {
      var stream = gulp.src(`${PROJECT_SOURCE_DIR}${folder}/*.js`) 
          .pipe(concat(`${folder}.js`)) 
          //.pipe(rename({ extname: ".min.js" })) 
        if (argv.environment=='production') stream = stream.pipe(uglify())
      return stream.pipe(gulp.dest(OUTPUT_DIR));
  });

  tasks.push(project_tasks);
  tasks.push(gulp.src('./source/scripts/config/'+argv.environment+'.js').pipe(concat('config.js')).pipe(gulp.dest('./public/scripts/')));
  return Promise.all(tasks);
});


gulp.task('styles', function () {
    return gulp.src(['../judotech.web/styles/**/*.css','./source/styles/**/*.css'])
        .pipe(gulp.dest('./public/styles'));
});

const imagemin = await import('gulp-imagemin').then((mod) => mod.default);
gulp.task('images', function () {
    return gulp.src(['../judotech.web/images/**/*.*','./source/images/**/*.*'], { since: gulp.lastRun('images') })
        .pipe(imagemin())
        .pipe(gulp.dest('./public/images'));
});

gulp.task('configurations', function () {
    return gulp.src('./source/configurations/**/*.js')
        .pipe(gulp.dest('./public/configurations'));
});

gulp.task('data', function () {
    return gulp.src('./source/data/**/*.json')
        .pipe(gulp.dest('./public/data'));
});

gulp.task('watch', function () {
    gulp.watch('source/pug/**/*.pug', gulp.series('pug'));
    gulp.watch('source/styles/**/*.css', gulp.series('styles'));
    gulp.watch('source/scripts/**/*.js', gulp.series('scripts'));
});

gulp.task('default', gulp.series('clean', 'pug', 'styles', 'scripts', 'images', 'configurations','data', function (done) {
    done();
}));
