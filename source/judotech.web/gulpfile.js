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
  const PROJECT_SOURCE_DIR = "source/scripts/";
  const OUTPUT_DIR = "public/scripts/";

  const project_folders = fs.readdirSync(PROJECT_SOURCE_DIR, { withFileTypes: true })
    .filter(dirent => dirent.isDirectory() && dirent.name != "config")
    .map(dirent => dirent.name);

  const tasks = project_folders.map(folder => {
      var stream = gulp.src(`${PROJECT_SOURCE_DIR}${folder}/*.js`)
          .pipe(concat(`${folder}.js`))
        if (argv.environment=='production') stream = stream.pipe(uglify())
      return stream.pipe(gulp.dest(OUTPUT_DIR));
  });

  tasks.push(
    gulp.src('./source/scripts/config/'+argv.environment+'.js', { allowEmpty: true })
      .pipe(concat('config.js'))
      .pipe(gulp.dest('./public/scripts/'))
  );
  return Promise.all(tasks);
});


gulp.task('styles', function () {
    return gulp.src('./source/styles/**/*.css', { allowEmpty: true })
        .pipe(gulp.dest('./public/styles'));
});

gulp.task('images', function () {
    return gulp.src('./source/images/**/*.*', { since: gulp.lastRun('images'), encoding: false, allowEmpty: true })
        .pipe(gulp.dest('./public/images'));
});

gulp.task('configurations', function (done) {
    if (!fs.existsSync('./source/configurations')) return done();
    return gulp.src('./source/configurations/**/*.js', { allowEmpty: true })
        .pipe(gulp.dest('./public/configurations'));
});

gulp.task('data', function (done) {
    if (!fs.existsSync('./source/data')) return done();
    return gulp.src('./source/data/**/*.json', { allowEmpty: true })
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
